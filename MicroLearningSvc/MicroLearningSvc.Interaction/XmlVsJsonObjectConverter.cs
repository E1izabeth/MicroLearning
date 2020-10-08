using System;
using SC = System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;

namespace MicroLearningSvc.Interaction
{
    public class XmlVsJsonObjectConverter
    {
        private const string _typeKeyName = "_type";

        private abstract class JsonTypeContract
        {
            public Type Type { get; private set; }

            public JsonTypeContract(Type type)
            {
                this.Type = type;

                if (type != null)
                {
                    _contractsByType.Add(type, this);
                    _contractsByName.Add(type.FullName, this);
                }
            }

            public object Deserialize(object dictionary)
            {
                if (dictionary == null)
                    return null;

                if (this.Type.IsAbstract)
                    throw new NotImplementedException("");

                return this.DeserializeImpl(dictionary);
            }

            protected abstract object DeserializeImpl(object data);


            public object Serialize(object obj)
            {
                if (obj == null)
                    return null;

                if (this.Type.IsAbstract)
                    throw new NotImplementedException("");

                return this.SerializeImpl(obj);
            }

            protected abstract object SerializeImpl(object objj);


            public IEnumerable<TsTypeDef> MakeTsType()
            {
                return this.MakeTsTypeImpl();
            }

            protected abstract IEnumerable<TsTypeDef> MakeTsTypeImpl();
        }

        private class ParseableTypeContract : JsonTypeContract
        {
            private readonly Func<object, object> _parsingAction;

            public ParseableTypeContract(Type type)
                : base(type)
            {
                var arg = Expression.Parameter(typeof(object), "obj");
                var expr = Expression.Lambda<Func<object, object>>(
                    Expression.ConvertChecked(Expression.Call(type, "Parse", null, Expression.ConvertChecked(arg, typeof(string))), typeof(object)),
                    arg
                );

                _parsingAction = expr.Compile();
            }

            protected override object SerializeImpl(object obj)
            {
                return obj;
            }

            protected override object DeserializeImpl(object data)
            {
                var type = data.GetType();
                if (type == this.Type)
                    return data;

                if (type == typeof(string))
                    return _parsingAction(data);

                return _parsingAction(data.ToString());
            }

            static readonly Type[] _numbericTypes = new[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
            };

            protected override IEnumerable<TsTypeDef> MakeTsTypeImpl()
            {
                var t = base.Type;

                if (t == typeof(string))
                    ; // nothing
                else if (t == typeof(bool))
                    ; // nothing
                else if (_numbericTypes.Contains(t))
                    yield return new TsUnionTypeDef(base.Type.Name, true, TsTypeSig.For(TsBuiltinTypeKind.Number));
                else
                {
                    if (!TsTypeSig.TryParseBuiltinTypeName(base.Type.Name, out var kind))
                        yield return new TsUnionTypeDef(base.Type.Name, true, TsTypeSig.For(TsBuiltinTypeKind.String));
                    else
                        throw new NotImplementedException();
                }
            }
        }

        private class StringTypeContract : JsonTypeContract
        {
            public StringTypeContract(Type type)
                : base(type) { }

            protected override object SerializeImpl(object obj)
            {
                return obj;
            }

            protected override object DeserializeImpl(object data)
            {
                return data;
            }

            protected override IEnumerable<TsTypeDef> MakeTsTypeImpl()
            {
                yield break;
            }
        }

        private abstract class JsonPropertyContract
        {
            public PropertyInfo Prop { get; private set; }
            public JsonTypeContract Owner { get; private set; }
            public JsonTypeContract Type { get; private set; }
            public string ElementName { get; protected set; }

            public JsonPropertyContract(JsonTypeContract owner, PropertyInfo prop)
            {
                this.Owner = owner;
                this.Prop = prop;
                this.Type = GetContractInternal(prop.PropertyType, true);
                this.ElementName = this.ResolveElementName(prop);
            }

            protected abstract string ResolveElementName(PropertyInfo prop);

            public void Deserialize(object instance, IDictionary<string, object> data)
            {
                this.DeserializeImpl(instance, data);
            }

            protected abstract void DeserializeImpl(object instance, IDictionary<string, object> data);

            public void Serialize(object obj, Dictionary<string, object> data)
            {
                this.SerializeImpl(obj, data);
            }

            protected abstract void SerializeImpl(object obj, Dictionary<string, object> data);

            public IEnumerable<TsTypeDef> MakeTsFieldDef(TsClassDef typeDef)
            {
                var result = this.MakeRequiredTypeDefsImpl(out var fieldTsType);
                typeDef.AddField(this.ElementName, fieldTsType);
                return result;
            }

            protected abstract TsTypeDef[] MakeRequiredTypeDefsImpl(out TsTypeSig fieldTsType);
        }

        private class AttributePropertyContract : JsonPropertyContract
        {
            public AttributePropertyContract(JsonTypeContract owner, PropertyInfo prop)
                : base(owner, prop) { }

            protected override string ResolveElementName(PropertyInfo prop)
            {
                var a = prop.GetCustomAttribute<XmlAttributeAttribute>();
                //if (a == null)
                //    throw new InvalidOperationException();
                if (a != null && a.Type != null)
                    throw new NotImplementedException("");

                // var name = a.AttributeName.IsNotEmpty() ? a.AttributeName : prop.Name;
                return prop.Name;
            }

            protected override void SerializeImpl(object obj, Dictionary<string, object> data)
            {
                data.Add(this.ElementName, this.Type.Serialize(this.Prop.GetValue(obj)));
            }

            protected override void DeserializeImpl(object instance, IDictionary<string, object> data)
            {
                object value;
                if (data.TryGetValue(this.ElementName, out value))
                    this.Prop.SetValue(instance, this.Type.Deserialize(value));
            }

            protected override TsTypeDef[] MakeRequiredTypeDefsImpl(out TsTypeSig fieldTsType)
            {
                fieldTsType = TsTypeSig.For(this.Type.Type.Name);
                return new TsTypeDef[0];
            }
        }

        private abstract class ElementPropertyContract : JsonPropertyContract
        {
            protected readonly Dictionary<JsonTypeContract, string> _nameByType = new Dictionary<JsonTypeContract, string>();
            protected readonly Dictionary<string, JsonTypeContract> _typeByName = new Dictionary<string, JsonTypeContract>();

            public ElementPropertyContract(JsonTypeContract owner, PropertyInfo prop)
                : base(owner, prop)
            {
                var attrs = prop.GetCustomAttributes<XmlElementAttribute>().ToArray();

                if (attrs.Length == 1)
                {
                    if (attrs.First().Type != null && attrs.First().Type != prop.PropertyType)
                        throw new NotImplementedException("");

                    // _nameByType.Add(this.Type, null);
                    // _typeByName.Add(string.Empty, this.Type);
                }
                else if (attrs.Length > 1)
                {
                    foreach (var a in attrs)
                    {
                        if (a.Type == null || a.ElementName.IsEmpty())
                            throw new NotImplementedException("");

                        var type = GetContractInternal(a.Type, true);

                        _nameByType.Add(type, a.ElementName);
                        _typeByName.Add(a.ElementName, type);
                    }
                }
                else
                {
                    //throw new NotImplementedException("");
                    // no element attribute specified, default behaviour: treat as element
                }
            }

            protected object SerializeValueInstance(object value)
            {
                var valueType = value.GetType();
                var contract = GetContractInternal(valueType, false);
                var instance = contract.Serialize(value);

                string typeName;
                if (contract is CustomTypeContract)
                {
                    if (!_nameByType.TryGetValue(contract, out typeName))
                    {
                        if (contract.Type != this.Type.Type)
                        {
                            //if (contract == _remoteObjectContract)
                            //{
                            //    typeName = _remoteObjectContract.Type.FullName;
                            //}
                            //else
                            //{
                            typeName = valueType.FullName;
                            //}
                        }
                        else
                        {
                            typeName = null;
                        }
                    }

                    if (typeName != null)
                    {
                        var dict = new Dictionary<string, object>();
                        dict.Add(_typeKeyName, typeName);
                        ((IDictionary<string, object>)instance).ForEach(kv => dict.Add(kv.Key, kv.Value));
                        instance = dict;
                    }
                }

                return instance;
            }

            protected object DeserializeValueInstance(object value)
            {
                if (value.GetType() == this.Prop.GetType())
                    return value;

                var data = value as IDictionary<string, object>;
                object instance;

                if (data != null)
                {
                    JsonTypeContract contract;

                    object typeNameObj;
                    if (data.TryGetValue(_typeKeyName, out typeNameObj))
                    {
                        var typeName = typeNameObj.ToString();
                        if (!_typeByName.TryGetValue(typeName ?? string.Empty, out contract))
                            if (!_contractsByName.TryGetValue(typeName, out contract))
                                throw new SerializationException($"Unexpected type name '{typeName}'. Expecting one of " + string.Join(", ", _typeByName.Keys.Select(k => $"'{k}'")));
                    }
                    else
                    {
                        contract = this.Type;
                    }

                    instance = contract.Deserialize(value);
                }
                else if (value is string && this.Type is ParseableTypeContract)
                {
                    instance = this.Type.Deserialize(value);
                }
                else
                {
                    instance = value;
                }

                return instance;
            }

            protected override string ResolveElementName(PropertyInfo prop)
            {
                return prop.Name;
            }

            protected override TsTypeDef[] MakeRequiredTypeDefsImpl(out TsTypeSig fieldTsType)
            {
                if (_typeByName.Count > 1)
                {
                    var typeName = this.Prop.DeclaringType.Name + "_" + this.ElementName;
                    var typeCases = _typeByName.Values.Select(s => TsTypeSig.For(s.Type.Name)).ToArray();
                    fieldTsType = TsTypeSig.For(typeName);
                    return new[] { new TsUnionTypeDef(typeName, true, typeCases) };
                }
                else
                {
                    if (this is MultiElementPropertyContract)
                    {
                        fieldTsType = TsTypeSig.For(this.Prop.PropertyType.GetElementType().Name);
                    }
                    else
                    {
                        fieldTsType = TsTypeSig.For(this.Prop.PropertyType.Name);
                    }
                    return new TsTypeDef[0];
                }
            }
        }

        private class SingleElementPropertyContract : ElementPropertyContract
        {
            public SingleElementPropertyContract(JsonTypeContract owner, PropertyInfo prop)
                : base(owner, prop) { }

            protected override void SerializeImpl(object obj, Dictionary<string, object> data)
            {
                var value = this.Prop.GetValue(obj);
                var instance = value == null ? null : this.SerializeValueInstance(value);
                data.Add(this.ElementName, instance);
            }

            protected override void DeserializeImpl(object instance, IDictionary<string, object> data)
            {
                object value;
                if (!data.TryGetValue(this.ElementName, out value))
                    value = null;
                var obj = value == null ? null : this.DeserializeValueInstance(value);
                this.Prop.SetValue(instance, obj);
            }
        }

        private class MultiElementPropertyContract : ElementPropertyContract
        {
            public MultiElementPropertyContract(JsonTypeContract owner, PropertyInfo prop)
                : base(owner, prop) { }

            protected override void SerializeImpl(object obj, Dictionary<string, object> data)
            {
                var value = this.Prop.GetValue(obj);

                var items = ((SC.IEnumerable)value)?.OfType<object>()
                                ?.Select(o => o == null ? null : this.SerializeValueInstance(o))
                                ?.ToArray();

                data.Add(this.ElementName, items);
            }

            protected override void DeserializeImpl(object instance, IDictionary<string, object> data)
            {
                object obj;
                object value;
                if (data.TryGetValue(this.ElementName, out value) && value != null)
                {
                    var items = ((object[])value).Select(o => o == null ? null : this.DeserializeValueInstance(o)).ToArray();

                    var t = this.Prop.PropertyType;
                    if (t.IsArray)
                    {
                        var arr = Array.CreateInstance(t.GetElementType(), items.Length);
                        for (int i = 0; i < items.Length; i++)
                            arr.SetValue(items[i], i);

                        obj = arr;
                    }
                    else if (typeof(SC.IList).IsAssignableFrom(this.Prop.PropertyType))
                    {
                        SC.IList list = (SC.IList)Activator.CreateInstance(this.Prop.PropertyType);
                        for (int i = 0; i < items.Length; i++)
                            list.Add(items[i]);

                        obj = list;
                    }
                    else
                    {
                        throw new SerializationException();
                    }

                    this.Prop.SetValue(instance, obj);
                }
            }

            protected override TsTypeDef[] MakeRequiredTypeDefsImpl(out TsTypeSig fieldTsType)
            {
                var result = base.MakeRequiredTypeDefsImpl(out var itemTsType);
                fieldTsType = TsTypeSig.ForArray(itemTsType);
                return result;
            }
        }

        private static JsonPropertyContract MakePropertyContract(JsonTypeContract owner, PropertyInfo prop)
        {
            var type = prop.PropertyType;
            JsonPropertyContract contract;

            if (IsParseableType(type) || type == typeof(string))
                contract = new AttributePropertyContract(owner, prop);
            else if (typeof(SC.IEnumerable).IsAssignableFrom(prop.PropertyType))
                contract = new MultiElementPropertyContract(owner, prop);
            else
                contract = new SingleElementPropertyContract(owner, prop);

            return contract;
        }

        private class CustomTypeContract : JsonTypeContract
        {
            private readonly ReadOnlyCollection<JsonPropertyContract> _props;
            private readonly Func<object> _ctor;

            public CustomTypeContract(Type type)
                   : base(type)
            {
                var props = type.GetProperties();
                var contracts = new List<JsonPropertyContract>(props.Length);

                props.Where(p => p.HasCustomAttribute<XmlAttributeAttribute>())
                    .ForEach(p => contracts.Add(new AttributePropertyContract(this, p)));
                props.Where(p => p.HasCustomAttribute<XmlElementAttribute>())
                    .OrderBy(p => p.GetCustomAttributes<XmlElementAttribute>().First().Order)
                    .ForEach(p => contracts.Add(MakePropertyContract(this, p)));
                props.Where(p => p.HasCustomAttribute<XmlArrayItemAttribute>())
                    .ForEach(p => contracts.Add(new MultiElementPropertyContract(this, p)));

                foreach (var item in props.Where(p => contracts.All(c => c.Prop != p)).Where(p => !p.HasCustomAttribute<XmlIgnoreAttribute>()))
                {
                    if (item.PropertyType.IsArray)
                        contracts.Add(new MultiElementPropertyContract(this, item));
                    else
                        contracts.Add(new SingleElementPropertyContract(this, item));
                }

                /*
                collect static serialization info
                    per property:
                        if scalar
                            if only concrete type
                                type name not needed
                            else if alternatives possible (inheritance)
                                type name required
                        if collection
                            if only concrete type
                                type name not needed
                            else if alternatives possible (inheritance)
                                type name required

                if XmlElementAttribute for type presented
                    use its name as type
                else if one XmlElementAttribute without type and instance type matches property
                    type not needed
                else
                    use instance type
                */


                _props = contracts.AsReadOnly();

                if (!type.IsAbstract)
                    _ctor = (Func<object>)Expression<Func<object>>.Lambda(Expression.Convert(Expression.New(type), typeof(object))).Compile();
            }

            protected override object SerializeImpl(object obj)
            {
                var data = new Dictionary<string, object>();

                _props.ForEach(p => p.Serialize(obj, data));

                return data;
            }

            protected override object DeserializeImpl(object obj)
            {
                var data = (Dictionary<string, object>)obj;
                var instance = _ctor();

                _props.ForEach(p => p.Deserialize(instance, data));

                return instance;
            }

            protected override IEnumerable<TsTypeDef> MakeTsTypeImpl()
            {
                var typeDef = new TsClassDef(this.Type.Name, true);

                foreach (var item in _props.SelectMany(p => p.MakeTsFieldDef(typeDef)))
                    yield return item;

                yield return typeDef;
            }
        }

        //class ProxiedTypeContract : CustomTypeContract
        //{
        //    public ProxiedTypeContract()
        //        : base(typeof(RpcRemoteObjectReferenceInfoType))
        //    {
        //    }

        //    protected override object SerializeImpl(object obj)
        //    {
        //        object data;
        //        if (obj is IRemoteObject)
        //        {
        //            var info = context.Marshal(obj);
        //            data = base.SerializeImpl(info, context);
        //        }
        //        else if (obj is RpcRemoteObjectReferenceInfoType)
        //        {
        //            data = base.Serialize(obj, context);
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException();
        //        }

        //        return data;
        //    }

        //    protected override object DeserializeImpl(object data)
        //    {
        //        var info = base.DeserializeImpl(data, context);
        //        var instance = context.Unmarshal(info);
        //        return instance;
        //    }
        //}

        private static readonly RWLock _lock = new RWLock();
        private static readonly Dictionary<Type, JsonTypeContract> _contractsByType = new Dictionary<Type, JsonTypeContract>();
        private static readonly Dictionary<string, JsonTypeContract> _contractsByName = new Dictionary<string, JsonTypeContract>();

        // static JsonTypeContract _remoteObjectContract = new ProxiedTypeContract();

        /*
        static void RegisterTypeContract(Type type)
        {
            if (!_contractsByType.ContainsKey(type))
            {
                if (type.GetCustomAttributes<XmlTypeAttribute>().Any())
                {
                    new CustomTypeContract(type);
                }
                else
                {
                    throw new NotImplementedException("");
                }
            }
        }*/

        private static bool IsParseableType(Type type)
        {
            return type.GetMethods().Any(m => m.IsStatic && m.Name == "Parse" && m.GetParameters().Length == 1 && m.GetParameters().All(p => p.ParameterType == typeof(string)));
        }

        private static JsonTypeContract GetContractInternal(Type type, bool createIfMissing)
        {
            JsonTypeContract contract;

            //if (typeof(IRemoteObject).IsAssignableFrom(type))
            //{
            //    contract = _remoteObjectContract;
            //}
            //else 
            if (!_contractsByType.TryGetValue(type, out contract))
            {
                if (!createIfMissing)
                    throw new InvalidOperationException();

                if (type.HasCustomAttribute<XmlTypeAttribute>())
                    contract = new CustomTypeContract(type);
                else if (IsParseableType(type))
                    contract = new ParseableTypeContract(type);
                else if (type == typeof(string))
                    contract = new StringTypeContract(type);
                else if (type == typeof(object))
                    contract = new CustomTypeContract(type);
                else if (type.IsArray)
                    contract = GetContractInternal(type.GetElementType(), createIfMissing);
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                    contract = GetContractInternal(type.GetGenericArguments().First(), createIfMissing);
                else if (typeof(SC.IEnumerable).IsAssignableFrom(type))
                    contract = GetContractInternal(typeof(object), createIfMissing);
                else
                    throw new NotImplementedException("");
            }

            return contract;
        }

        private static JsonTypeContract GetContract(Type type)
        {
            JsonTypeContract contract = null;

            if (contract == null)
                using (_lock.Read())
                    contract = GetContractInternal(type, false);

            if (contract == null)
                using (_lock.Write())
                    contract = GetContractInternal(type, true);

            return contract;
            // throw new SerializationException("Missing contract for type " + type + "! Consider treat it is as supporting during converter instantiation.");
        }

        private readonly ReadOnlyCollection<Type> _supportedTypes;

        public IEnumerable<Type> SupportedTypes { get { return _supportedTypes; } }

        public XmlVsJsonObjectConverter(params Type[] types)
        {
            _supportedTypes = types.AsReadOnly();

            using (_lock.Write())
            {
                types.SelectMany(t => ContractHelpers.CollectContractTypes(t)).Distinct().ForEach(t => GetContractInternal(t, true));
            }
        }

        public static IEnumerable<TsTypeDef> CollectTypeScriptMapping(params Type[] types)
        {
            var conv = new XmlVsJsonObjectConverter(types);
            return _contractsByType.Values.SelectMany(c => c.MakeTsType());
        }

        /*
        public IDictionary<string,object> ToDictionary(object obj)
        {
            return GetContract(type).Deserialize(dictionary);
        }

        public override object FromDictionary(IDictionary<string,object> data)
        {
            return obj == null ? null : GetContract(obj.GetType()).Serialize(obj);
        }
        */

        public object FromTree(object tree, Type type = null)
        {
            if (tree == null)
                return null;

            var data = (Dictionary<string, object>)tree;
            JsonTypeContract contract;

            if (type != null)
            {
                contract = GetContract(type);
            }
            else
            {
                if (!_contractsByName.TryGetValue(data[_typeKeyName].ToString(), out contract))
                    throw new SerializationException("Unknown type name");
            }

            return contract.Deserialize(data);
        }

        public object ToTree(object obj, bool skipRootType = false)
        {
            if (obj == null)
                return null;

            var contract = GetContract(obj.GetType());
            return contract.Serialize(obj);
        }
    }

    static class ContractHelpers
    {
        public static IEnumerable<Type> CollectContractTypes(Type type, HashSet<Type> knownTypes = null)
        {
            if (knownTypes == null)
                knownTypes = new HashSet<Type>();

            if (type != null && type.GetCustomAttributes<XmlTypeAttribute>().Any() && knownTypes.Add(type))
            {
                yield return type;

                foreach (XmlIncludeAttribute item in type.GetCustomAttributes<XmlIncludeAttribute>())
                    foreach (var subitem in CollectContractTypes(item.Type, knownTypes))
                        yield return subitem;

                foreach (var prop in type.GetProperties())
                {
                    foreach (var subitem in CollectContractTypes(prop.PropertyType, knownTypes))
                        yield return subitem;

                    foreach (XmlElementAttribute item in prop.GetCustomAttributes<XmlElementAttribute>())
                        foreach (var subitem in CollectContractTypes(item.Type, knownTypes))
                            yield return subitem;
                }
            }
        }
    }

    public class RWLock
    {
        enum LockState
        {
            Locking,
            Locked,
            Unlockring,
            Unlocked
        }

        abstract class Lock : IDisposable
        {
            public LockState State { get; private set; }

            protected readonly RWLock _lock;
            protected readonly int _threadId;

            public Lock(RWLock @lock)
            {
                _threadId = Thread.CurrentThread.ManagedThreadId;
                _lock = @lock;
                this.State = LockState.Locking;
            }

            protected void SetLocked()
            {
                this.State = LockState.Locked;
            }

            protected void SetUnlocking()
            {
                this.State = LockState.Unlockring;
            }

            public virtual void Dispose()
            {
                this.State = LockState.Unlocked;
            }
        }

        class ReaderLock : Lock
        {
            public ReaderLock(RWLock @lock)
                : base(@lock)
            {
                _lock.AcquireReaderLock();
                this.SetLocked();
            }

            public override void Dispose()
            {
                this.SetUnlocking();
                _lock.ReleaseReaderLock(this);
                base.Dispose();
            }
        }

        class WriterLock : Lock
        {
            public WriterLock(RWLock @lock)
                : base(@lock)
            {
                _lock.AcquireWriterLock();
                this.SetLocked();
            }

            public override void Dispose()
            {
                this.SetUnlocking();
                _lock.ReleaseWriterLock(this);
                base.Dispose();
            }
        }

        ReaderWriterLock _lock = new ReaderWriterLock();
        List<Lock> _lockers = new List<Lock>();

        public RWLock()
        {
        }

        private void AcquireReaderLock()
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
        }

        private void ReleaseReaderLock(ReaderLock locker)
        {
            _lock.ReleaseReaderLock();
            lock (_lockers) _lockers.Remove(locker);
        }

        private void AcquireWriterLock()
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
        }

        private void ReleaseWriterLock(WriterLock locker)
        {
            _lock.ReleaseWriterLock();
            lock (_lockers) _lockers.Remove(locker);
        }

        public IDisposable Write()
        {
            var locker = new WriterLock(this);
            lock (_lockers) _lockers.Add(locker);
            return locker;
        }

        public IDisposable Read()
        {
            var locker = new ReaderLock(this);
            lock (_lockers) _lockers.Add(locker);
            return locker;
        }
    }

    public enum TsBuiltinTypeKind
    {
        Number,
        Boolean,
        String,
        Void,
        Null,
        Undefined,
        Any
    }

    public interface ITsFormattable
    {
        void FormatTo(IndentedWriter writer);
    }

    public abstract class TsCodePart : ITsFormattable
    {
        public void FormatTo(IndentedWriter writer)
        {
            this.FormatToImpl(writer);
        }

        protected abstract void FormatToImpl(IndentedWriter writer);

        public string FormatToString()
        {
            var sw = new IndentedWriter();
            this.FormatTo(sw);
            return sw.GetContentAsString();
        }
    }

    public abstract class TsTypeSig : TsCodePart
    {
        public static TsTypeSig For(string name) { return TryParseBuiltinTypeName(name, out var kind) ? new TsBuiltinTypeSig(kind) : (TsTypeSig)new TsCustomTypeSig(name); }
        public static TsTypeSig For(TsBuiltinTypeKind kind) { return new TsBuiltinTypeSig(kind); }
        public static TsTypeSig ForGeneric(string name, params TsTypeSig[] args) { return new TsCustomGenericTypeSig(name, args); }
        public static TsTypeSig ForArray(TsTypeSig itemTsType) { return new TsArrayTypeSig(itemTsType); }

        private static readonly string[] _builtinTypeName = Enum.GetNames(typeof(TsBuiltinTypeKind)).Select(s => s.ToLower()).ToArray();

        public static bool IsBuiltinTypeName(string name) { return _builtinTypeName.Contains(name); }
        public static bool TryParseBuiltinTypeName(string name, out TsBuiltinTypeKind kind) { return Enum.TryParse<TsBuiltinTypeKind>(name, out kind); }

        public override string ToString() { return this.FormatToString(); }
    }

    public class TsCustomTypeSig : TsTypeSig
    {
        public string Prefix { get; }
        public string Name { get; }

        public TsCustomTypeSig(string name)
            : this(null, name) { }

        public TsCustomTypeSig(string prefix, string name)
        {
            this.Prefix = prefix;
            this.Name = name;
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(this.Prefix))
            {
                writer.Write(this.Prefix);
                writer.Write(".");
            }

            writer.Write(this.Name);
        }
    }

    public class TsBuiltinTypeSig : TsTypeSig
    {
        public TsBuiltinTypeKind Kind { get; }

        public TsBuiltinTypeSig(TsBuiltinTypeKind kind)
        {
            this.Kind = kind;
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            writer.Write(this.Kind.ToString().ToLower());
        }
    }

    public class TsCustomGenericTypeSig : TsCustomTypeSig
    {
        public ReadOnlyCollection<TsTypeSig> GenericParameters { get; }

        public TsCustomGenericTypeSig(string name, params TsTypeSig[] args)
            : this(null, name, args) { }

        public TsCustomGenericTypeSig(string prefix, string name, params TsTypeSig[] args)
            : base(prefix, name)
        {
            this.GenericParameters = new ReadOnlyCollection<TsTypeSig>(args);
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            base.FormatToImpl(writer);
            writer.Write("<");

            var it = this.GenericParameters.GetEnumerator();
            if (it.MoveNext())
            {
                it.Current.FormatTo(writer);

                while (it.MoveNext())
                {
                    writer.Write(", ");
                    it.Current.FormatTo(writer);
                }
            }

            writer.Write(">");
        }
    }

    public class TsArrayTypeSig : TsCustomGenericTypeSig
    {
        public TsArrayTypeSig(TsTypeSig itemTypeSig)
            : base(null, "Array", itemTypeSig) { }
    }

    public abstract class TsTypeDef : TsCodePart
    {
        public bool IsExport { get; }
        public string Name { get; }

        public TsTypeDef(string name, bool export)
        {
            this.Name = name;
            this.IsExport = export;
        }
    }

    public enum TsMemberAccessKind
    {
        Default,
        Public,
        Protected,
        Private
    }

    public abstract class TsTypeMemberDef : TsCodePart
    {
        public string Name { get; }
        public TsMemberAccessKind Access { get; }

        public TsTypeMemberDef(string name, TsMemberAccessKind access)
        {
            this.Name = name;
            this.Access = access;
        }

        protected void FormatAccessMode(IndentedWriter writer)
        {
            if (this.Access != TsMemberAccessKind.Default)
                writer.Write(this.Access.ToString().ToLower());
        }
    }

    public class TsClassFieldDef : TsTypeMemberDef
    {
        public TsTypeSig Type { get; }
        public bool IsOptional { get; }

        public TsClassFieldDef(string name, TsTypeSig type, TsMemberAccessKind access = TsMemberAccessKind.Default, bool optional = false)
            : base(name, access)
        {
            this.Type = type;
            this.IsOptional = optional;
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            base.FormatAccessMode(writer);
            writer.Write(this.Name);

            if (this.IsOptional)
                writer.Write("?");

            writer.Write(": ");
            this.Type.FormatTo(writer);
            writer.Write(";");
        }
    }

    public class TsClassDef : TsTypeDef
    {
        readonly Dictionary<string, TsTypeMemberDef> _members = new Dictionary<string, TsTypeMemberDef>();

        public TsClassDef(string name, bool export)
            : base(name, export)
        {
        }

        public void AddField(string name, TsTypeSig type, TsMemberAccessKind access = TsMemberAccessKind.Default)
        {
            _members.Add(name, new TsClassFieldDef(name, type, access));
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            if (this.IsExport)
                writer.Write("export ");

            writer.Write("class ").Write(this.Name);

            writer.WriteLine(" {").Push();

            foreach (var member in _members)
            {
                member.Value.FormatTo(writer);
                writer.WriteLine();
            }

            writer.Pop().WriteLine("}");
        }
    }

    public class TsUnionTypeDef : TsTypeDef
    {
        readonly List<TsTypeSig> _cases;

        public ReadOnlyCollection<TsTypeSig> Cases { get { return new ReadOnlyCollection<TsTypeSig>(_cases.ToArray()); } }

        public TsUnionTypeDef(string name, bool export, params TsTypeSig[] cases)
            : base(name, export)
        {
            _cases = new List<TsTypeSig>(cases);
        }

        public void Add(TsTypeSig type)
        {
            _cases.Add(type);
        }

        protected override void FormatToImpl(IndentedWriter writer)
        {
            if (this.IsExport)
                writer.Write("export ");

            writer.Write("type ");
            writer.Write(this.Name);
            writer.Write(" = ");

            var it = _cases.GetEnumerator();
            if (it.MoveNext())
            {
                it.Current.FormatTo(writer);

                while (it.MoveNext())
                {
                    writer.Write(" | ");
                    it.Current.FormatTo(writer);
                }
            }

            writer.WriteLine(";");
        }
    }
}

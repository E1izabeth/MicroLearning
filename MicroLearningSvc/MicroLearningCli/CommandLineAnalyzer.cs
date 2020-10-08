using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningCli
{
    /// <summary>
    /// Describes alternative name for argument
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ArgAliasAttribute : Attribute
    {
        public string Alias { get; private set; }

        public ArgAliasAttribute(string alias)
        {
            this.Alias = alias;
        }
    }

    /// <summary>
    /// Describes required argument order
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultArgAttribute : Attribute
    {
        public int Order { get; private set; }

        public DefaultArgAttribute(int order)
        {
            this.Order = order;
        }
    }

    /// <summary>
    /// Describes argument help message
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ArgDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public ArgDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }

    public class CommandLineAnalyzer<T>
    {
        class ArgDescr
        {
            public string Alias { get; private set; }
            public string Key { get; private set; }
            public PropertyInfo Property { get; private set; }
            public bool IsDefault { get; private set; }

            public ArgDescr(PropertyInfo prop, bool @default = true)
            {
                this.Property = prop;
                this.IsDefault = @default;

                if (!@default)
                    this.Key = "--" + PropToArgName(prop);

                var attribs = prop.GetCustomAttributes(typeof(ArgAliasAttribute), false);
                if (attribs != null && attribs.Length > 0)
                {
                    this.Alias = "-" + ((ArgAliasAttribute)attribs[0]).Alias;
                }
            }

            public string GetHelp()
            {
                string ret;

                var attribs = this.Property.GetCustomAttributes(typeof(ArgDescriptionAttribute), false);
                if (attribs != null && attribs.Length > 0)
                {
                    ret = ((ArgDescriptionAttribute)attribs[0]).Description;
                }
                else
                {
                    ret = null;
                }

                return ret;
            }
        }

        static readonly Dictionary<string, ArgDescr> _optionalArgs = new Dictionary<string, ArgDescr>();
        static readonly List<ArgDescr> _optionalArgsList = new List<ArgDescr>();
        static readonly List<ArgDescr> _defaultArgsList = new List<ArgDescr>();

        static CommandLineAnalyzer()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var attribs = prop.GetCustomAttributes(typeof(DefaultArgAttribute), false);
                if (attribs != null && attribs.Length > 0)
                {
                    _defaultArgsList.Insert(((DefaultArgAttribute)attribs[0]).Order, new ArgDescr(prop));
                }
                else
                {
                    var descr = new ArgDescr(prop, false);
                    _optionalArgs.Add(descr.Key, descr);
                    _optionalArgsList.Add(descr);

                    if (descr.Alias != null)
                        _optionalArgs.Add(descr.Alias, descr);
                }
            }
        }

        static string PropToArgName(PropertyInfo prop)
        {
            var name = prop.Name;
            var n = name.Length;
            var argName = string.Empty;

            for (int i = name.Length - 1; i > 0; i--)
            {
                if (char.IsUpper(name[i]))
                {
                    argName = "-" + char.ToLower(name[i]) + name.Substring(i + 1, n - i - 1) + argName;
                    n = i;
                }
            }

            argName = char.ToLower(name[0]) + name.Substring(1, n - 1) + argName;
            return argName;
        }

        public string MakeHelp()
        {
            var sb = new StringBuilder();

            sb.Append("Usage : \n\t\t")
              .Append(typeof(T).Assembly.ManifestModule.Name);

            if (_optionalArgs.Count > 0)
                sb.Append(" [options]");


            foreach (var arg in _defaultArgsList)
            {
                sb.Append(" <").Append(arg.Property.Name).Append(">");
            }

            sb.Append("\n\nwhere\n");

            foreach (var arg in _defaultArgsList)
            {
                var help = arg.GetHelp();
                if (help != null)
                {
                    sb.Append("\t<").Append(arg.Property.Name).Append(">");
                    FormatHelpDescr(sb, help);
                }
            }

            if (_optionalArgs.Count > 0)
            {
                sb.Append("\noptions are\n");

                foreach (var arg in _optionalArgsList)
                {
                    var help = arg.GetHelp();
                    if (help != null)
                    {
                        if (arg.Alias != null)
                            sb.Append("\t").Append(arg.Alias).Append("\n");

                        sb.Append("\t").Append(arg.Key);
                        FormatHelpDescr(sb, help);
                    }
                }
            }

            return sb.ToString();
        }

        static void FormatHelpDescr(StringBuilder sb, string str)
        {
            var w = Console.BufferWidth - 17;
            foreach (var item in str.Split('\n'))
            {
                var line = item;
                sb.Append("\n\t\t");

                var n = 0;
                while (line.Length > w)
                {
                    var l = Math.Min(w, line.Length);

                    sb.Append(line.Substring(n, l))
                      .Append("\n\t\t");

                    n += l;
                    line = line.Substring(n);
                }

                sb.Append(line);
            }
            sb.AppendLine();
        }

        public string ErrorMessage { get; private set; }
        public int FoundDefaultArgs { get; private set; }
        public bool AllDefaultArgsFound { get; private set; }

        public string[] RestArguments { get; private set; }

        public CommandLineAnalyzer()
        {
            this.ErrorMessage = null;
            this.FoundDefaultArgs = 0;
            this.AllDefaultArgsFound = false;
        }

        public bool TryParse(string[] args, T options)
        {
            return this.TryParse(args, options, false);
        }

        public bool TryParse(string[] args, T options, bool captureRest)
        {
            int i = 0;

            for (; i < args.Length; i++)
            {
                ArgDescr arg;
                if (!_optionalArgs.TryGetValue(args[i], out arg))
                    break;

                if (!this.TryParseArgument(args, ref i, arg, options))
                    return false;
            }

            int argNum = 0;
            for (; i < args.Length && _defaultArgsList.Count > 0 && argNum < _defaultArgsList.Count;)
            {
                if (!this.TryParseArgument(args, ref i, _defaultArgsList[argNum], options))
                    return false;

                argNum++;
            }

            this.FoundDefaultArgs = argNum;

            if (captureRest)
            {
                this.RestArguments = args.Skip(i).ToArray(); // TODO: why it was (i - 1) here?
                i += this.RestArguments.Length;
            }

            var ok = (i >= args.Length && argNum >= _defaultArgsList.Count);
            if (!ok)
                this.ErrorMessage = "Invalid number of arguments";

            this.AllDefaultArgsFound = ok;

            return true;
        }

        private bool TryParseArgument(string[] args, ref int index, ArgDescr arg, T options)
        {
            var type = arg.Property.PropertyType;

            if (type == typeof(string))
            {
                var argIndex = arg.IsDefault ? index++ : ++index;
                var str = args[argIndex];
                
                if (str.StartsWith("\""))
                    str = str.Substring(1, str.Length - 2);

                arg.Property.SetValue(options, str, null);
            }
            else if (type == typeof(long))
            {
                var argIndex = arg.IsDefault ? index++ : ++index;
                arg.Property.SetValue(options, long.Parse(args[argIndex]), null);
            }
            else if (type == typeof(bool))
            {
                arg.Property.SetValue(options, true, null);
            }
            else
            {
                this.ErrorMessage = "Unknown command line argument type [" + type.FullName + "]";
                return false;
            }

            return true;
        }
    }
}

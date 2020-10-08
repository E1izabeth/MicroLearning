using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Util
{
    class IlOp
    {
        public int Offset { get; }
        public OpCode OpCode { get; }

        public IlOp(int offset, OpCode opCode)
        {
            this.OpCode = opCode;
        }
    }

    class IlOp<T> : IlOp
    {
        public T Arg { get; }

        public IlOp(int offset, OpCode opCode, T arg)
            : base(offset, opCode)
        {
            this.Arg = arg;
        }
    }


    class IlParser
    {
        static readonly OpCode[] _ops = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public)
                                               .Select(f => (OpCode)f.GetValue(null))
                                               .ToArray();

        class IlContext
        {
            public MethodBase Method { get; private set; }
            public MethodBody Body { get; private set; }
            public Module Module { get; private set; }
            public ModuleHandle ModuleHandle { get; private set; }

            public int Position { get; private set; }
            public bool EndOfMethodReached { get { return this.Position >= _il.Length; } }

            byte[] _il;
            Type[] _genericTypeArguments;
            Type[] _genericMethodArguments;

            public IlContext(MethodBase m)
            {
                this.Method = m;
                this.Body = m.GetMethodBody();
                this.Module = m.Module;
                this.ModuleHandle = m.Module.ModuleHandle;
                this.Position = 0;

                _il = this.Body.GetILAsByteArray();
                _genericTypeArguments = this.Method.DeclaringType.IsGenericType ? this.Method.DeclaringType.GetGenericArguments() : null;
                _genericMethodArguments = this.Method.IsGenericMethod ? this.Method.GetGenericArguments() : null;
            }

            public byte CurrUInt8 { get { return _il[this.Position]; } }
            public short CurrInt16 { get { return BitConverter.ToInt16(_il, this.Position); } }
            public int CurrInt32 { get { return BitConverter.ToInt32(_il, this.Position); } }
            public long CurrInt64 { get { return BitConverter.ToInt64(_il, this.Position); } }
            public float CurrSingle { get { return BitConverter.ToSingle(_il, this.Position); } }
            public double CurrDouble { get { return BitConverter.ToDouble(_il, this.Position); } }

            public void Advance(int n)
            {
                var next = this.Position + n;
                //if (next < 0 || n >= _il.Length)
                //    throw new NotImplementedException("");
                if (this.EndOfMethodReached)
                    throw new NotImplementedException("");

                this.Position = next;
            }

            public MethodBase ResolveMethod(int token)
            {
                return this.Module.ResolveMethod(token, _genericTypeArguments, _genericMethodArguments);
            }

            public FieldInfo ResolveField(int token)
            {
                return this.Module.ResolveField(token, _genericTypeArguments, _genericMethodArguments);
            }

            public Type ResolveType(int token)
            {
                return this.Module.ResolveType(token, _genericTypeArguments, _genericMethodArguments);
            }

            public MemberInfo ResolveToken(int token)
            {
                return this.Module.ResolveMember(token, _genericTypeArguments, _genericMethodArguments);
            }

            public byte[] ResolvesSignature(int token)
            {
                return this.Module.ResolveSignature(token);
            }

            public string ResolveString(int token)
            {
                return this.Module.ResolveString(token);
            }
        }

        private static IlOp MakeOp(int offset, OpCode opCode)
        {
            return new IlOp(offset, opCode);
        }

        private static IlOp<T> MakeOp<T>(int offset, OpCode opCode, T arg)
        {
            return new IlOp<T>(offset, opCode, arg);
        }

        public static IEnumerable<IlOp> Translate(MethodBase m)
        {
            var sb = new StringBuilder();
            var c = new IlContext(m);

            while (!c.EndOfMethodReached)
            {
                short code = c.CurrUInt8;
                var off = c.Position;

                c.Advance(1);
                var oo = _ops.First(op => op.Value == code);

                if (oo.OpCodeType == OpCodeType.Nternal)
                {
                    code = (short)(code << 8 | c.CurrUInt8);
                    oo = _ops.First(op => op.Value == code);
                    c.Advance(1);
                }

                string argStr;

                switch (oo.OperandType)
                {
                    case OperandType.InlineNone:
                        {
                            argStr = string.Empty;
                            yield return MakeOp(off, oo);
                        }
                        break;
                    case OperandType.InlineTok:
                        {
                            var arg = c.ResolveToken(c.CurrInt32);
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineMethod:
                        {
                            var arg = c.ResolveMethod(c.CurrInt32);
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineField:
                        {
                            var arg = c.ResolveField(c.CurrInt32);
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineString:
                        {
                            var arg = c.ResolveString(c.CurrInt32);
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineType:
                        {
                            var arg = c.ResolveType(c.CurrInt32);
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineSig:
                        {
                            var arg = c.ResolvesSignature(c.CurrInt32);
                            argStr = arg.ToString();
                            //ilw.WriteOpCode(ilOp, tokens.MakeTokenIndex(TranslateSignature(arg)));
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineI:
                        {
                            var arg = oo.OperandType == OperandType.InlineBrTarget ? c.CurrInt32 + off : c.CurrInt32;
                            argStr = Convert.ToString(arg, 16).PadLeft(8, '0');
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                        {
                            var arg = oo.OperandType == OperandType.ShortInlineBrTarget ? c.CurrUInt8 + off : c.CurrUInt8;
                            argStr = Convert.ToString(arg, 16).PadLeft(8, '0');
                            yield return MakeOp(off, oo, arg);
                            c.Advance(1);
                        }
                        break;
                    case OperandType.ShortInlineVar:
                        {
                            // argStr = m.GetMethodBody().LocalVariables[c.CurrUInt8].ToString();
                            argStr = "$" + c.CurrUInt8;
                            yield return MakeOp(off, oo, c.CurrUInt8);
                            c.Advance(1);
                        }
                        break;
                    case OperandType.InlineVar:
                        {
                            // argStr = m.GetMethodBody().LocalVariables[c.CurrInt16].ToString();
                            argStr = "$" + c.CurrInt16;
                            yield return MakeOp(off, oo, c.CurrInt16);
                            c.Advance(2);
                        }
                        break;
                    case OperandType.InlineSwitch:
                        {
                            var cnt = c.CurrInt32;
                            var branches = new int[cnt];
                            c.Advance(4);
                            argStr = "(";
                            if (cnt > 0)
                            {
                                argStr += Convert.ToString(c.CurrInt32, 16).PadLeft(8, '0');
                                branches[0] = c.CurrInt32;
                                c.Advance(4);
                                for (int j = 1; j < cnt; j++)
                                {
                                    branches[j] = c.CurrInt32;
                                    argStr += ", ";
                                    argStr += Convert.ToString(c.CurrInt32, 16).PadLeft(8, '0');
                                    c.Advance(4);
                                }
                            }
                            argStr += ")";
                            yield return MakeOp(off, oo, branches);
                        }
                        break;
                    case OperandType.InlineI8:
                        {
                            var arg = c.CurrInt64;
                            argStr = Convert.ToString(arg, 16).PadLeft(16, '0');
                            yield return MakeOp(off, oo, arg);
                            c.Advance(8);
                        }
                        break;
                    case OperandType.ShortInlineR:
                        {
                            var arg = c.CurrSingle;
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(4);
                        }
                        break;
                    case OperandType.InlineR:
                        {
                            var arg = c.CurrDouble;
                            argStr = arg.ToString();
                            yield return MakeOp(off, oo, arg);
                            c.Advance(8);
                        }
                        break;
                    default:
                        throw new NotImplementedException(oo.ToString());
                }

                var line = string.Format("    L_{0}: {1}{2}", Convert.ToString(off, 16).PadLeft(4, '0'), oo.ToString().PadRight(16, ' '), argStr);
                sb.AppendLine(line);
            }
        }

        //private static MethodSignatureDefinition TranslateSignature(byte[] sig)
        //{
        //    throw new NotImplementedException("");
        //}
    }
}

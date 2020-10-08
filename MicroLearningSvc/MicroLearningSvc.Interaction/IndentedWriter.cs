using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Interaction
{
    public class IndentedWriter
    {
        StringBuilder _builder = new StringBuilder();
        int _indent = 0;
        bool _lineHasIndent = false;

        public string IndentString { get; private set; }

        public IndentedWriter(string indentString = "\t")
        {
            this.IndentString = indentString ?? "\t";
        }

        private void EnsureIndent()
        {
            if (!_lineHasIndent)
            {
                for (int i = 0; i < _indent; i++)
                    _builder.Append(this.IndentString);

                _lineHasIndent = true;
            }
        }

        public IndentedWriter Write(string line)
        {
            this.EnsureIndent();
            _builder.Append(line);
            return this;
        }

        public IndentedWriter Write(string format, params object[] args)
        {
            this.EnsureIndent();
            _builder.AppendFormat(format, args);
            return this;
        }

        public IndentedWriter WriteLine()
        {
            this.EnsureIndent();
            _builder.AppendLine();
            _lineHasIndent = false;
            return this;
        }

        public IndentedWriter WriteLine(string line)
        {
            this.EnsureIndent();
            _builder.AppendLine(line);
            _lineHasIndent = false;
            return this;
        }

        public IndentedWriter WriteLine(string format, params object[] args)
        {
            this.EnsureIndent();
            _builder.AppendFormat(format, args);
            _builder.AppendLine();
            _lineHasIndent = false;
            return this;
        }

        public IndentedWriter Push()
        {
            _indent++;

            return this;
        }

        public IndentedWriter Pop()
        {
            if (_indent <= 0)
                throw new InvalidOperationException();

            _indent--;

            return this;
        }

        public string GetContentAsString()
        {
            return _builder.ToString();
        }
    }
}

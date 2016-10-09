using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public interface IStatementReader
    {
        string ReadStatement(TextReader reader);
    }
    public abstract class StatementReader : IStatementReader
    {
        protected readonly Func<char, bool> _readCondition;
        public StatementReader(Func<char, bool> readCondition)
        {
            this._readCondition = readCondition;
        }

        public virtual string ReadStatement(TextReader textReader)
        {
            return this.ReadWhile(textReader, this._readCondition);
        }
        
        protected string ReadWhile(TextReader textReader, Func<char, bool> readCondition)
        {
            var expression = string.Empty;
            int peek;
            while ((peek = textReader.Peek()) > -1)
            {
                var next = (char)peek;
                if (readCondition(next))
                {
                    expression += (char)textReader.Read();
                }
                else
                {
                    break;
                }
            }
            return expression;
        }

        protected void SkipOneChar(TextReader textReader)
        {
            if (textReader.Peek() > -1) { textReader.Read(); }
        }
    }
}

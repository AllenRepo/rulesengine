using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public interface IStatementParser
    {
        bool CanParse(char @char);
        Statement ParseStatement(TextReader textReader, Expression param);
    }
    public abstract class StatementParser : IStatementParser
    {
        protected readonly IStatementReader _expressionReader;
        protected readonly Func<char, bool> _parseCondition;
        public StatementParser(IStatementReader expressionReader, Func<char, bool> parseCondition)
        {
            this._expressionReader = expressionReader;
            this._parseCondition = parseCondition;
        }

        public virtual bool CanParse(char @char) { return _parseCondition(@char); }
        public abstract Statement ParseStatement(TextReader textReader, Expression param);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class QuotationParser : StatementParser
    {
        public QuotationParser(IStatementReader expressionReader) : base(expressionReader, (@char) => @char == '"') { }

        public override Statement ParseStatement(TextReader textReader, Expression param)
        {
            string statement = base._expressionReader.ReadStatement(textReader);
            return Statement.CreateStatement(typeof(string), Expression.Constant(statement));
        }
    }
}

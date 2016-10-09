using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class NumericParser : StatementParser
    {
        public NumericParser(IStatementReader expressionReader) : base(expressionReader, char.IsDigit) { }

        public override Statement ParseStatement(TextReader textReader, Expression param)
        {
            decimal result;
            string expression = base._expressionReader.ReadStatement(textReader);
            if (decimal.TryParse(expression, out result))
            {
                return Statement.CreateStatement(typeof(decimal), Expression.Constant(result));
            }
            else
            {
                throw new InvalidCastException($"Unable to convert {expression} to numeric.");
            }
        }
    }
}

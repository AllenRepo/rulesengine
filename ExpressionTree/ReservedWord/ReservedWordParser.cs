using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class ReservedWordParser : StatementParser
    {

        public ReservedWordParser(IStatementReader expressionReader) : base(expressionReader, char.IsLetter) { }

        public override Statement ParseStatement(TextReader reader, Expression param)
        {
            string expression = this._expressionReader.ReadStatement(reader);
            return (ReservedWord)expression;
        }
    }
}

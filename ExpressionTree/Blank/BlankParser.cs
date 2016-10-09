using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class BlankParser : StatementParser
    {
        public BlankParser(IStatementReader expressionReader) : base(expressionReader, @char => @char == ' ') { }

        public override Statement ParseStatement(TextReader textReader, Expression param)
        {
            base._expressionReader.ReadStatement(textReader);
            return Statement.CreateStatement(null);
        }
    }
}

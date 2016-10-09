using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class ParamParser : StatementParser
    {
        public ParamParser(IStatementReader expressionReader) : base(expressionReader, (@char) => @char == '[') { }
        public override Statement ParseStatement(TextReader textReader, Expression param)
        {
            string statement = base._expressionReader.ReadStatement(textReader);
            var keyParam = Expression.Constant(statement);
            var result = Expression.Parameter(typeof(object), "result");
            var block = Expression.Block(
                new[] { result }, 
                Expression.Assign(result, Expression.Property(param, "Item", keyParam)), 
                result
            );

            return Statement.CreateStatement(typeof(string), block);
        }
    }
}

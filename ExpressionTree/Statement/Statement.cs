using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class Statement
    {
        public Type StatementType { get; private set; }
        public Expression Expression { get; private set; }
        public bool IsEmpty { get; private set; }
        protected Statement(Type statementType, Expression expression)
        {
            this.StatementType = statementType;
            this.IsEmpty = expression == null;
            this.Expression = expression;
        }
        public static Statement CreateStatement(Type statementType, Expression expression)
        {
            return new Statement(statementType, expression);
        }
        public static Statement CreateStatement(Expression expression)
        {
            return CreateStatement(typeof(object), expression);
        }
    }
}

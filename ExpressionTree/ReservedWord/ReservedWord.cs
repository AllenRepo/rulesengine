using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class ReservedWord : Statement
    {
        public static readonly ReservedWord True = new ReservedWord(typeof(bool), Expression.Constant(true));
        public static readonly ReservedWord False = new ReservedWord(typeof(bool), Expression.Constant(false));
        public static readonly ReservedWord Null = new ReservedWord(typeof(object), Expression.Constant(null));

        public static readonly Dictionary<string, ReservedWord> Words = new Dictionary<string, ReservedWord>
        {
            { "true", True },
            { "false", False },
            { "null", Null }
        };

        public ReservedWord(Type statementType, Expression expression) : base(statementType, expression) { }

        public static explicit operator ReservedWord(string symbol)
        {
            ReservedWord result;

            if (Words.TryGetValue(symbol, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class Operation
    {
        private readonly Func<Expression, Expression, Expression> _binaryOperation;
        private readonly Func<Expression, Expression> _unaryOperation;

        public int NumberOfOperands { get; private set; }
        public Type RequiredType { get; private set; }
        public int Precedence { get; private set; } //precedence rules from C

        public static readonly Operation Negate = new Operation(2, Expression.Negate, typeof(decimal));
        public static readonly Operation Multiplication = new Operation(3, Expression.Multiply, typeof(decimal));
        public static readonly Operation Division = new Operation(3, Expression.Divide, typeof(decimal));
        public static readonly Operation Addition = new Operation(4, Expression.Add, typeof(decimal));
        public static readonly Operation Subtraction = new Operation(4, Expression.Subtract, typeof(decimal));
        public static readonly Operation LessThan = new Operation(6, Expression.LessThan, typeof(decimal));
        public static readonly Operation LessThanOrEqual = new Operation(6, Expression.LessThanOrEqual, typeof(decimal));
        public static readonly Operation GreaterThan = new Operation(6, Expression.GreaterThan, typeof(decimal));
        public static readonly Operation GreaterThanOrEqual = new Operation(6, Expression.GreaterThanOrEqual, typeof(decimal));
        public static readonly Operation EqualTo = new Operation(7, Expression.Equal, null);
        public static readonly Operation NotEqualTo = new Operation(7, Expression.NotEqual, null);
        public static readonly Operation LogicalAnd = new Operation(11, Expression.AndAlso, null);
        public static readonly Operation LogicalOr = new Operation(12, Expression.OrElse, null);
        public static readonly Operation Assign = new Operation(14, Expression.Assign, null);
        public static readonly Operation LeftParanthesis = new Operation(1, Expression.Empty);
        public static readonly Operation RightParanthesis = new Operation(1, Expression.Empty);

        public static readonly Dictionary<string, Operation> Operations = new Dictionary<string, Operation>
        {
            { "*", Multiplication },
            { "/", Division },
            { "+", Addition },
            { "-", Subtraction },
            { "<", LessThan },
            { "<=", LessThanOrEqual },
            { ">", GreaterThan },
            { ">=", GreaterThanOrEqual },
            { "==", EqualTo },
            { "!=", NotEqualTo },
            { "&&", LogicalAnd },
            { "||", LogicalOr },
            { "=", Assign },
            { "(", LeftParanthesis },
            { ")", RightParanthesis }
        };
        public static readonly List<char> FirstLevelSymbol = Operations.Select(x => x.Key[0]).ToList();
        public static readonly List<char> SecondLevelSymbol = Operations.Where(x => x.Key.Length > 1).Select(x => x.Key[1]).ToList();
        public static readonly List<char> ThirdLevelSymbol = Operations.Where(x => x.Key.Length > 2).Select(x => x.Key[2]).ToList();

        private Operation(int precedence, Type requiredType)
        {
            this.Precedence = precedence;
            this.RequiredType = requiredType;
        }

        private Operation(int precedence, Func<Expression> emptyOperation) : this(precedence, typeof(object)) { }

        private Operation(int precedence, Func<Expression, Expression> unaryOperation, Type requiredType) : this(precedence, requiredType)
        {
            this._unaryOperation = unaryOperation;
            this.NumberOfOperands = 1;
        }

        private Operation(int precedence, Func<Expression, Expression, Expression> operation, Type requiredType) : this(precedence, requiredType)
        {
            this._binaryOperation = operation;
            this.NumberOfOperands = 2;
        }

        public Expression Apply(params Expression[] expressions)
        {
            return this.NumberOfOperands == 1
                ? _unaryOperation(expressions[0]) 
                : _binaryOperation(expressions[0], expressions[1]);
        }

        public static bool IsOperation(string operation)
        {
            return Operations.ContainsKey(operation);
        }

        public static explicit operator Operation(string operation)
        {
            Operation result;

            if (Operations.TryGetValue(operation, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException($"Unknown operator {operation}.");
            }
        }
    }
}

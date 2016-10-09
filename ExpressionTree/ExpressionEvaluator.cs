using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class ExpressionEvaluator
    {
        private readonly Stack<Statement> _statementStack = new Stack<Statement>();
        private readonly Stack<Operation> _operationStack = new Stack<Operation>();

        private readonly IEnumerable<IStatementParser> _statementParsers = new IStatementParser[]
        {
            new ReservedWordParser(new ReservedWordReader()),
            new NumericParser(new NumericReader()),
            new QuotationParser(new QuotationReader()),
            new BlankParser(new BlankReader()),
            new ParamParser(new ParamReader())
        };
        private readonly OperationParser _operationParser = new OperationParser(new OperationReader());

        public Func<T, bool> Compile<T>(string expression)
        {
            var param = Expression.Parameter(typeof(T), "param");
            using (var reader = new StringReader(expression))
            {
                int peek;
                while ((peek = reader.Peek()) > -1)
                {
                    var next = (char)peek;

                    var parser = _statementParsers.FirstOrDefault(x => x.CanParse(next));
                    if (parser != null)
                    {
                        var statement = parser.ParseStatement(reader, param);
                        if (!statement.IsEmpty)
                        {
                            _statementStack.Push(statement);
                        }
                    }
                    else if (this._operationParser.CanParse(next))
                    {
                        var currentOperation = this._operationParser.ParseOperation(reader);
                        if (currentOperation == Operation.LeftParanthesis)
                        {
                            _operationStack.Push(currentOperation);
                            continue;
                        }
                        else if (currentOperation == Operation.RightParanthesis)
                        {
                            EvaluateWhile(() => _operationStack.Count > 0 && _operationStack.Peek() != Operation.LeftParanthesis);
                            _operationStack.Pop();
                        }
                        else
                        {
                            EvaluateWhile(() => _operationStack.Count > 0 && currentOperation.Precedence >= _operationStack.Peek().Precedence && _operationStack.Peek() != Operation.LeftParanthesis);
                            _operationStack.Push(currentOperation);
                        }
                    }
                    else
                    {
                        throw new Exception("critical error");
                    }
                }
            }

            EvaluateWhile(() => _operationStack.Count > 0);

            //var lambda = Expression.Lambda<Func<T, bool>>(_expressionStack.Pop(), dictionaryParameter);
            var lambda = Expression.Lambda<Func<T, bool>>(_statementStack.Pop().Expression, param);
            return lambda.Compile();
        }
        
        public Func<bool> Build(string expression)
        {
            //var dictionaryParameter = Expression.Parameter(typeof(T), "args");

            using (var reader = new StringReader(expression))
            {
                int peek;
                while((peek = reader.Peek()) > -1)
                {
                    var next = (char)peek;

                    var parser = _statementParsers.FirstOrDefault(x => x.CanParse(next));
                    if (parser != null)
                    {
                        var statement = parser.ParseStatement(reader, null);
                        if (!statement.IsEmpty) {
                            _statementStack.Push(statement);
                        }
                    }
                    else if (this._operationParser.CanParse(next))
                    {
                        var currentOperation = this._operationParser.ParseOperation(reader);
                        if (currentOperation == Operation.LeftParanthesis)
                        {
                            _operationStack.Push(currentOperation);
                            continue;
                        }
                        else if (currentOperation == Operation.RightParanthesis)
                        {
                            EvaluateWhile(() => _operationStack.Count > 0 && _operationStack.Peek() != Operation.LeftParanthesis);
                            _operationStack.Pop();
                        }
                        else
                        {
                            EvaluateWhile(() => _operationStack.Count > 0 && currentOperation.Precedence >= _operationStack.Peek().Precedence);
                            _operationStack.Push(currentOperation);
                        }
                        
                    }
                    else
                    {
                        throw new Exception("critical error");
                    }
                }
            }

            EvaluateWhile(() => _operationStack.Count > 0);

            //var lambda = Expression.Lambda<Func<T, bool>>(_expressionStack.Pop(), dictionaryParameter);
            var lambda = Expression.Lambda<Func<bool>>(_statementStack.Pop().Expression);
            return lambda.Compile();
        }

        private void EvaluateWhile(Func<bool> condition)
        {
            while (condition())
            {
                var operation = _operationStack.Pop();

                var expressions = new Expression[operation.NumberOfOperands];
                for (var i = operation.NumberOfOperands - 1; i >= 0; i--)
                {
                    var expression = _statementStack.Pop().Expression;
                    expressions[i] = operation.RequiredType == typeof(decimal) && expression.Type != typeof(decimal)
                        ? Expression.Call(null, typeof(Convert).GetMethod("ToDecimal", new Type[] { typeof(object) }), expression)
                        : expression;
                }
                _statementStack.Push(Statement.CreateStatement(typeof(object), operation.Apply(expressions)));
            }
        }
    }
}

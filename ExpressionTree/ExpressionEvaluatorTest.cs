using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine.ExpressionTree
{
    [TestClass]
    public class ExpressionEvaluatorTest
    {
        [TestMethod]
        public void should_handle_logical_and()
        {
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build("2+3>=1+7");
            var result = expression();

            var expression2 = evaluator.Build("2+1<100");
            var result2 = expression2();
        }

        [TestMethod]
        public void should_evaluate_complex_logical_and()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build("true && true");
            var expression2 = evaluator.Build("true && false");
            var expression3 = evaluator.Build("false && false");
            //Act
            var result = expression();
            var result2 = expression2();
            var result3 = expression3();
            //Assert
            Assert.AreEqual(result, true && true);
            Assert.AreEqual(result2, true && false);
            Assert.AreEqual(result3, false && false);
        }

        [TestMethod]
        public void should_evaluate_complex_logical_or()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build("true || true");
            var expression2 = evaluator.Build("true || false");
            var expression3 = evaluator.Build("false || false");
            //Act
            var result = expression();
            var result2 = expression2();
            var result3 = expression3();
            //Assert
            Assert.AreEqual(result, true || true);
            Assert.AreEqual(result2, true || false);
            Assert.AreEqual(result3, false || false);
        }

        [TestMethod]
        public void should_evaluate_complex_logical_and_or()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build("true && false || true");
            var expression2 = evaluator.Build("true || false && true");
            var expression3 = evaluator.Build("true || false || true && true && false");
            //Act
            var result = expression();
            var result2 = expression2();
            var result3 = expression3();
            //Assert
            Assert.AreEqual(result, true && false || true);
            Assert.AreEqual(result2, true || false && true);
            Assert.AreEqual(result3, true || false || true && true && false);
        }

        [TestMethod]
        public void should_evaulate_singular_boolean_condition()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build("true");
            var expression2 = evaluator.Build("false");
            //Act
            var result = expression();
            var result2 = expression2();
            //Assert
            Assert.AreEqual(result, true);
            Assert.AreEqual(result2, false);
        }

        [TestMethod]
        public void should_evaulate_string_equivalence_comparision()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var expression = evaluator.Build(@"""word"" == ""word""");
            var expression2 = evaluator.Build(@"""word"" != ""word""");
            var expression3 = evaluator.Build(@"""word"" == ""word2""");
            var expression4 = evaluator.Build(@"""word"" != ""word2""");
            //Act
            var result = expression();
            var result2 = expression2();
            var result3 = expression3();
            var result4 = expression4();
            //Assert
            Assert.AreEqual(result, "word".Equals("word"));
            Assert.AreEqual(result2, !"word".Equals("word"));
            Assert.AreEqual(result3, "word".Equals("word2"));
            Assert.AreEqual(result4, !"word".Equals("word2"));
        }

        [TestMethod]
        public void should_handle_dictionary_parameters()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var dictionary = new Dictionary<string, string> { { "word", "word" } };
            var expression = evaluator.Compile<Dictionary<string, string>>(@"""word"" == [word]");
            //Act
            var result = expression(dictionary);
            //Assert
            Assert.AreEqual(result, "word".Equals(dictionary["word"]));
        }

        [TestMethod]
        public void should_eval_complex_expression()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var dictionary = new Dictionary<string, string> { { "a", "20" }, { "b", null } };
            var expression = evaluator.Compile<Dictionary<string, string>>(@"""20"" == [a] && 20 > 5");
            var expression2 = evaluator.Compile<Dictionary<string, string>>("null == [b]");
            //Act
            var result = expression(dictionary);
            var result2 = expression2(dictionary);
            //Assert
            Assert.AreEqual(result, "20".Equals(dictionary["a"]) && 20 > 5);
            Assert.AreEqual(result2, dictionary["b"] == null);
        }

        [TestMethod]
        public void should_eval_complex_type()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var dictionary = new Dictionary<string, object> { { "a", 20m }, { "b", "100" } };
            var expression = evaluator.Compile<Dictionary<string, object>>("20 > [a]");
            var expression2 = evaluator.Compile<Dictionary<string, object>>("20 > [b]");
            //Act
            var result = expression(dictionary);
            var result2 = expression(dictionary);
            //Assert
            Assert.AreEqual(result, 20 > (decimal)dictionary["a"]);
            Assert.AreEqual(result2, 20 > Convert.ToDecimal(dictionary["b"]));
        }

        [TestMethod]
        public void should_eval_paranthesis()
        {
            //Arrange
            var evaluator = new ExpressionEvaluator();
            var dictionary = new Dictionary<string, object> { { "a", 20m }, { "b", "100" } };
            var expression = evaluator.Compile<Dictionary<string, object>>("(1 + 2) * 3 > 10");
            var expression2 = evaluator.Compile<Dictionary<string, object>>("(1 + [b]) * 2 > 100");
            //Act
            var result = expression(dictionary);
            var result2 = expression2(dictionary);
            //Assert
            Assert.AreEqual(result, (1 + 2) * 3 > 10);
            Assert.AreEqual(result2, (1 + Convert.ToDecimal(dictionary["b"])) * 2 > 100);
        }

        [TestMethod]
        public void change_type_test()
        {
            var method = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) });
            var result = method.Invoke(null, new object[] { 20, typeof(decimal) });
        }

        [TestMethod]
        public void change_type_test2()
        {
            var expression = Expression.Call(null, typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }), Expression.Constant("20", typeof(object)), Expression.Constant(typeof(decimal)));
            var lambda = Expression.Lambda<Func<object>>(expression);
            var result = lambda.Compile()();

            var expression2 = Expression.Call(null, typeof(Convert).GetMethod("ToString", new Type[] { typeof(int) }), Expression.Constant(20));
            var lambda2 = Expression.Lambda<Func<string>>(expression2);
            var result2 = lambda2.Compile()();
        }

        [TestMethod]
        public void change_type_test3()
        {
            var param = Expression.Parameter(typeof(Person), "person");
            var property = typeof(Person).GetProperty("Value");
            var method = typeof(Convert).GetMethod("ToDecimal", new Type[] { typeof(object) });
            var value = Expression.Call(null, method, Expression.Property(param, property));

            var expression = Expression.GreaterThan(value, Expression.Constant(100m));
            var lambda = Expression.Lambda<Func<Person, bool>>(expression, param);
            var compiled = lambda.Compile();
            var result = compiled(new Person { Value = "20" });
            var result2 = compiled(new Person { Value = "1000" });
        }

        public class Person
        {
            public string Value { get; set; }
        }

        [TestMethod]
        public void test_if_then_assignment()
        {
            var param = Expression.Parameter(typeof(string), "param");
            var expression = Expression.Assign(param, Expression.Constant("Hello!"));
            var lambda = Expression.Lambda<Action<string>>(expression, param);
            var compiled = lambda.Compile();
            
            var result = string.Empty;
            compiled(result);


            var fn = (Action<string>)((s) => s = "Hello!");
        }

        [TestMethod]
        public void test_dictionary_assignment()
        {
            var keyParam = Expression.Parameter(typeof(string), "key");
            var param = Expression.Parameter(typeof(Dictionary<string, string>), "param");
            var valueParam = Expression.Parameter(typeof(string), "value");
            var block = Expression.Block(
                    Expression.Assign(Expression.Property(param, "Item", keyParam), valueParam)
                );
            var expression = Expression.Lambda<Action<Dictionary<string, string>, string, string>>(block, param, keyParam, valueParam);
            var compiled = expression.Compile();

            var dictionary = new Dictionary<string, string> { { "a", "a" } };
            compiled(dictionary, "a", "Hello");
        }

        [TestMethod]
        public void test_object_assignment()
        {
            var param = Expression.Parameter(typeof(Person), "param");
            var trueParam = Expression.Parameter(typeof(string), "value");
            var falseParam = Expression.Parameter(typeof(string), "value");
            var block = Expression.Block(
                    Expression.IfThenElse(Expression.GreaterThan(Expression.Constant(2), Expression.Constant(1)), 
                        Expression.Assign(Expression.Property(param, "Value"), trueParam),
                        Expression.Assign(Expression.Property(param, "Value"), falseParam)
                    )
                );
            var expression = Expression.Lambda<Action<Person, string, string>>(block, param, trueParam, falseParam);
            var compiled = expression.Compile();

            var person = new Person { Value = "a" };
            compiled(person, "true", "false");
        }

        [TestMethod]
        public void test_assignment()
        {
            ParameterExpression xparam = Expression.Parameter(typeof(Person), "x");
            ParameterExpression yparam = Expression.Parameter(typeof(Person), "y");
            BinaryExpression body = Expression.Assign(yparam, xparam);
            var expr = Expression.Lambda<Action<Person, Person>>(body, xparam, yparam);
            var cexpr = expr.Compile();
            var a = new Person { Value = "a" };
            var b = new Person { Value = "b" };
            cexpr(a, b);

            Action<Person> action = (va) => va.Value = "Hello";
            var v = new Person();
            action(v);
        }

        public class Helper
        {
            public void DoSomething(string msg)
            {
                System.Diagnostics.Debug.WriteLine("Helper executed;");
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        [TestMethod]
        public void test_helper()
        {
            var helper = Expression.Parameter(typeof(Helper), "Helper");
            var msgParam = Expression.Parameter(typeof(string), "msgParam");
            var expression = Expression.Call(helper, helper.Type.GetMethod("DoSomething"), msgParam);
            var lambda = Expression.Lambda(expression, helper, msgParam);
            var compiled = (Action<Helper, string>)lambda.Compile();

            var helperObj = new Helper();
            var msg = "Test";
            compiled(helperObj, msg);
            
        }

        [TestMethod]
        public void test_factorial()
        {
            var factorialParam = Expression.Parameter(typeof(int), "factorialParam");
            var resultParam = Expression.Parameter(typeof(int), "result");
            var label = Expression.Label(typeof(int));
            var expression = Expression.Block(
                new[] { resultParam },
                Expression.Assign(resultParam, Expression.Constant(1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThan(factorialParam, Expression.Constant(1)),
                        Expression.MultiplyAssign(resultParam, Expression.PostDecrementAssign(factorialParam)),
                        Expression.Break(label, resultParam)
                    ), label)
                );

            var labmda = Expression.Lambda(expression, factorialParam);
            var compiled = (Func<int, int>)labmda.Compile();

            var result = compiled(3);
            var result2 = compiled(4);
            var result3 = compiled(5);
            var result4 = compiled(6);
        }

        public static void TwiceTheInputByRef(ref int x)
        {
            x = x * 2;
        }

        [TestMethod]
        public void test_reference_update()
        {
            var inputVar = Expression.Variable(typeof(int), "input");
            var blockExp = Expression.Block(
                new[] { inputVar }
                , Expression.Assign(inputVar, Expression.Constant(10))
                , Expression.Call(
                    typeof(ExpressionEvaluatorTest).GetMethod(
                        "TwiceTheInputByRef", new[] { typeof(int).MakeByRefType() }),
                    inputVar)
                , inputVar);
            var lambda = Expression.Lambda(blockExp, inputVar);
            var compiled = (Func<int, int>)lambda.Compile();
            
            var temp = 1;
            var result = compiled(temp);
        }

        public event Log Logged;
        public delegate void Log(string s);
        [TestMethod]
        public void delegates_and_events()
        {
            var del = (Log)(x => System.Diagnostics.Debug.WriteLine(x));
            Logged += del;
            Logged += del;
            Logged += s => System.Diagnostics.Debug.WriteLine(s + "yo");
            Logged("yo");
        }
        
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace RulesEngine.ExpressionTree
{
    [TestClass]
    public class functionTest
    {
        private readonly Dictionary<string, Type> MetaData = new Dictionary<string, Type> { { "Helper", typeof(Helper)} };
        [TestMethod]
        public void TestMethod1()
        {
            //Expression.Assign(instance, Expression.Property(param, "Item", Expression.Constant("Helper")))
            //Expression.Call(instance, instance.Type.GetMethod("Log"))
            //Expression.Call(Expression.Convert(Expression.Property(param, "Item", Expression.Constant("Helper")), typeof(Helper)), typeof(Helper).GetMethod("Log"))
            //Expression.Assign(instance, Expression.Convert(Expression.Property(param, "Item", Expression.Constant("Helper")), typeof(Helper))),
            string expression = "Helper.Log()";
            var param = Expression.Parameter(typeof(Dictionary<string, object>), "param");
            var block = Expression.Block(
                    Expression.Call(Expression.Convert(Expression.Property(param, "Item", Expression.Constant("Helper")), MetaData["Helper"]), MetaData["Helper"].GetMethod("Log"))
                );
            var lambda = Expression.Lambda(block, param);
            var compiled = (Action<Dictionary<string, object>>)lambda.Compile();

            var dictionary = new Dictionary<string, object> { { "Helper", new Helper() } };
            compiled(dictionary);
        }
    }

    public class Helper
    {
        public void Log()
        {
            System.Diagnostics.Debug.WriteLine("Hello World!");
        }
    }
}

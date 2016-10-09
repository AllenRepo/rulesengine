using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;

namespace RulesEngine.Roslyn
{
    [TestClass]
    public class MyCompilerTest
    {
        [TestMethod]
        public async Task should_perform_simple_math()
        {
            //Arrange
            var fn = Compiler.Build<SomeObject, int>(@"MyProperty + 20");
            //Act
            var result = await fn(new SomeObject());
            //Assert
            Assert.AreEqual(new SomeObject().MyProperty + 20, result);
        }

        [TestMethod]
        public async Task should_perform_simple_operation()
        {
            //Arrange
            var fn = Compiler.Build<SomeObject, int>(@"PassAndReturn(MyProperty)");
            //Act
            var result = await fn(new SomeObject());
            //Assert
            Assert.AreEqual(new SomeObject().MyProperty, result);
        }

        [TestMethod]
        public async Task should_perform_complex_operation()
        {
            //Arrange
            var fn = Compiler.Build<SomeObject, int>(@"var num = PassAndReturn(MyProperty); return num += PassAndReturn(MyProperty);");
            //Act
            var result = await fn(new SomeObject());
            //Assert
            Assert.AreEqual(new SomeObject().MyProperty + new SomeObject().MyProperty, result);
        }
    }

    public class SomeObject
    {
        public int MyProperty { get; set; } = 10;
        public int PassAndReturn(int value)
        {
            return value;
        }
    }
}

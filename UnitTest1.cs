using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Reflection;

namespace RulesEngine.Roslyn
{
    //reference https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void roslyn_sample_csharpscriptengine()
        {
            CSharpScriptEngine.Execute(
            //This could be code submitted from the editor
            @"
            using System;
            public class ScriptedClass
            {
                public String HelloWorld {get;set;}
                public ScriptedClass()
                {
                    HelloWorld = ""Hello Roslyn!"";
                }
            }");
            //And this from the REPL
            var result = CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld");
        }

        [TestMethod]
        public async Task TestParameter()
        {
            var script = CSharpScript.Create<string>(@"X + ""this""", globalsType: typeof(Globals));
            script.Compile();
            var result = await script.RunAsync(new Globals { X = 1 });
        }

        public class Globals
        {
            public int X;
            public int Y;
        }
        [TestMethod]
        public async Task roslyn_sample_csharpscript()
        {
            var globals = new Globals { X = 1, Y = 2 };
            var result = await CSharpScript.EvaluateAsync<int>("X+Y", globals: globals);
        }

        [TestMethod]
        public void roslyn_sample_syntax_tree()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
            using System;
            using System.Diagnostics;
            namespace RoslynCompileSample
            {
                public class Writer
                {
                    public void Write(string message)
                    {
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                }
            }");

            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());

                    Type type = assembly.GetType("RoslynCompileSample.Writer");
                    object obj = Activator.CreateInstance(type);
                    type.InvokeMember("Write",
                        BindingFlags.Default | BindingFlags.InvokeMethod,
                        null,
                        obj,
                        new object[] { "Hello World" });
                }
            }


        }
    }
    public class CSharpScriptEngine
    {
        private static ScriptState<object> scriptState = null;
        public static object Execute(string code)
        {
            scriptState = scriptState == null ? CSharpScript.RunAsync(code).Result : scriptState.ContinueWithAsync(code).Result;
            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                return scriptState.ReturnValue;
            return null;
        }
    }
}

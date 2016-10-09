using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.Roslyn
{
    public static class Compiler
    {
        public static Action<TInput> Build<TInput>(string code)
        {
            var script = CSharpScript.Create<TInput>(code, globalsType: typeof(TInput));
            script.Compile();
            return async (p) => await script.RunAsync(p);
        }

        public static Func<TInput, Task<TOutput>> Build<TInput, TOutput>(string code)
            where TInput : class
        {
            var script = CSharpScript.Create<TOutput>(code, globalsType: typeof(TInput));
            script.Compile();
            return async (p) =>
            {
                var response = await script.RunAsync(p);
                return response.ReturnValue;
            };
        }
    }
}

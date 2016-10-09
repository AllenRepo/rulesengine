using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class ParamReader : StatementReader
    {
        public ParamReader() : base((@char) => @char != ']') { }
        public override string ReadStatement(TextReader textReader)
        {
            base.SkipOneChar(textReader);
            string result = base.ReadWhile(textReader, base._readCondition);
            base.SkipOneChar(textReader);
            return result;
        }
    }
}

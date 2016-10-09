using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class OperationReader
    {
        public static Operation GetOperation(TextReader reader)
        {
            var operation = Parse(reader, Operation.FirstLevelSymbol.Contains, Operation.SecondLevelSymbol.Contains, Operation.ThirdLevelSymbol.Contains);
            return (Operation)operation;
        }

        private static string Parse(TextReader reader, params Func<char, bool>[] conditions)
        {
            string result = string.Empty;
            foreach(var condition in conditions)
            {
                if (condition((char)reader.Peek()))
                {
                    result += ((char)reader.Read());
                }
                else { break; }
            }
            return result;
        }

        public string ReadOperation(TextReader textReader)
        {
            string result = string.Empty;
            foreach (var condition in new Func<char, bool>[] { Operation.FirstLevelSymbol.Contains, Operation.SecondLevelSymbol.Contains, Operation.ThirdLevelSymbol.Contains })
            {
                if (condition((char)textReader.Peek()))
                {
                    result += ((char)textReader.Read());
                }
                else { break; }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class OperationParser
    {
        private readonly OperationReader _operationReader;

        public OperationParser(OperationReader operationReader)
        {
            this._operationReader = operationReader;
        }

        public bool CanParse(char @char)
        {
            return Operation.FirstLevelSymbol.Contains(@char);
        }

        public Operation ParseOperation(TextReader textReader)
        {
            string operation = this._operationReader.ReadOperation(textReader);
            return (Operation)operation;
        }
    }
}

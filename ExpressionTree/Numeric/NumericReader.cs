using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine.ExpressionTree
{
    public class NumericReader : StatementReader
    {
        public NumericReader() : base((@char) => char.IsDigit(@char) || @char == '.') { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class Node
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DataType DataType { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Value}";
        }
    }
}

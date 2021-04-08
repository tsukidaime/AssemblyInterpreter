using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class InterpreterService
    {
        public List<Node> Data { get; set; }
        private List<string> parameters;
        public InterpreterService()
        {
            Data = new List<Node>();
        }
        public bool IsOperation(string value)
        {
            foreach (var operation in Enum.GetValues(typeof(OperationType)).Cast<OperationType>())
            {
                if (value == operation.ToString()) return true;
            }
            return false;
        }

        private OperationType ConvertToOperationType(string operation)
        {
            if (Enum.TryParse(OperationType.ADD.GetType(), operation, out object? op)) return (OperationType)op;
            else throw new ArgumentException();
        }

        private (DataType, bool) ConvertToDataType(char type) => (type switch
        {
            'h' => (DataType.hex, true),
            'q' => (DataType.oct, true),
            'd' => (DataType.dec, true),
            'b' => (DataType.bin, true),
            _ => (DataType.dec, false),
        });

        public dynamic Execute(string operation, List<string> parameters)
        {
            var op = ConvertToOperationType(operation);
            this.parameters = parameters;
            dynamic res = op switch
            {
                OperationType.ADD => Add(),
                OperationType.INC => Inc(),
                OperationType.MUL => Mul(),
                OperationType.MOV => Mov(),
                OperationType.SUB => Sub(),
                OperationType.XCHG => Xchg(),
                OperationType.DIV => Div(),
                OperationType.DD => Create(parameters, string.Empty),
                OperationType.DW => Create(parameters, string.Empty),
                OperationType.DB => Create(parameters, string.Empty),
                OperationType.DEC => Dec(),
                _ => throw new ArgumentException(),
            };
            return res;
        }

        public string Create(List<string> parameters, string varName)
        {
            var isArray = (parameters.ToList().Count > 1);
            if (!isArray) {
                var res = RemoveRagix(parameters.First(), ConvertToDataType(parameters.First().Last()));
                var node = new Node
                {
                    Name = varName,
                    Value = res,
                    DataType = ConvertToDataType(parameters.First().Last()).Item1
                };
                Data.Add(node);
                return node.ToString();
            }
            else
            {
                var nodes = parameters.Select(x => RemoveRagix(x, ConvertToDataType(x.Last())));
                var res = string.Empty;
                foreach (var item in nodes)
                {
                    var node = new Node
                    {
                        Name = varName,
                        Value = item,
                        DataType = ConvertToDataType(parameters.First().Last()).Item1
                    };
                    Data.Add(node);
                    res += node.ToString() + " ";
                }
                return res;
            }
        }

        private string RemoveRagix(string value, (DataType,bool) type)
        {
            return type.Item2 ? value.Substring(0, value.Length - 1) : value;
        }

        private string Mov()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            firstVariable.Value = secondVariable.Value;
            return firstVariable.Value;
        }
        private string Xchg()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            var temp = firstVariable.Value;
            firstVariable.Value = secondVariable.Value;
            secondVariable.Value = temp;
            return $"{firstVariable.Name} - {firstVariable.Value} \t {secondVariable.Name} - {secondVariable.Value}";
        }

        private string Sub()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            var firstValue = Convert.ToInt32(firstVariable.Value, (int)firstVariable.DataType);
            var secondValue = Convert.ToInt32(secondVariable.Value, (int)secondVariable.DataType);
            return Convert.ToString(firstValue - secondValue, (int)secondVariable.DataType);
        }

        private string Div()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            var firstValue = Convert.ToInt32(firstVariable.Value, (int)firstVariable.DataType);
            var secondValue = Convert.ToInt32(secondVariable.Value, (int)secondVariable.DataType);
            return Convert.ToString(firstValue / secondValue, (int)secondVariable.DataType);
        }

        private string Mul()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            var firstValue = Convert.ToInt32(firstVariable.Value, (int)firstVariable.DataType);
            var secondValue = Convert.ToInt32(secondVariable.Value, (int)secondVariable.DataType);
            return Convert.ToString(firstValue * secondValue, (int)secondVariable.DataType);
        }

        private string Add()
        {
            var firstVariable = Data.Find(x => x.Name == parameters.First());
            var secondVariable = Data.Find(x => x.Name == parameters.Last());
            var firstValue = Convert.ToInt32(firstVariable.Value, (int)firstVariable.DataType);
            var secondValue = Convert.ToInt32(secondVariable.Value, (int)secondVariable.DataType);
            return Convert.ToString(firstValue + secondValue, (int)secondVariable.DataType);
        }

        private string Dec()
        {
            var variable = Data.Find(x => x.Name == parameters.First());
            var toDecrement = Convert.ToInt32(variable.Value, (int)variable.DataType);
            variable.Value = Convert.ToString(--toDecrement, (int)variable.DataType);
            return variable.Value;
        }

        private string Inc()
        {
            var variable = Data.Find(x => x.Name == parameters.First());
            var toIncrement = Convert.ToInt32(variable.Value, (int)variable.DataType);
            variable.Value =  Convert.ToString(++toIncrement, (int)variable.DataType) ;
            return variable.Value;
        }
    }
}


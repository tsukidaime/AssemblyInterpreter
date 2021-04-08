using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssemblyInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new InterpreterService();
            var lines = File.ReadAllLines(@"C:\Users\Aibek_Shulembekov\Downloads\assemblycode.txt");
            foreach(var line in lines)
            {
                var words = new List<string>(line.Split(' '));
                var firstValue = words[0];
                if (service.IsOperation(firstValue))
                {
                    var parameters = words.Skip(1).ToList();
                    Console.WriteLine(service.Execute(firstValue, parameters));
                }
                else
                {
                    var varName = firstValue;
                    var operation = words[1];
                    var parameters = words.Skip(2);
                    Console.WriteLine(service.Create(parameters.ToList(), varName));
                }
            }
        }
    }
}

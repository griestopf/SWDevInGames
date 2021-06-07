using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalcApp
{
    public interface IOperation
    {
        char Symbol { get; }
        int Operate(int l, int r);
    }

    public class Plus : IOperation
    {
        public char Symbol => '+';

        public int Operate(int l, int r)
        {
            return l + r;
        }
    }

    public class Minus : IOperation
    {
        public char Symbol => '-';

        public int Operate(int l, int r)
        {
            return l - r;
        }
    }


    public class Mult : IOperation
    {
        public char Symbol => '*';

        public int Operate(int l, int r)
        {
            return l * r;
        }
    }


    public class Div : IOperation
    {
        public char Symbol => '/';

        public int Operate(int l, int r)
        {
            return l / r;
        }
    }

 

    class Calculator
    {
        public List<IOperation> operations; 

        public string Calculate(string command)
        {
            int left;
            int right;
            char opSymbol;
            int opInx = FindFirstNonDigit(command);

            if (opInx < 0)
            {
                return "No operator specified";
            }

            try
            {
                left = int.Parse(command.Substring(0, opInx));
                right = int.Parse(command.Substring(opInx + 1));
            }
            catch (Exception)
            {
                return "Error parsing command";
            }
            opSymbol = command[opInx];

            foreach(IOperation op in operations)
            {
                if (opSymbol == op.Symbol)
                    return op.Operate(left, right).ToString();
            }

            return "Unknown operation '" + opSymbol + "'!";
        }


        private static int FindFirstNonDigit(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!Char.IsDigit(s[i]))
                    return i;
            }
            return -1;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator
            {
                operations = new List<IOperation>
                {
                    new Plus(), new Minus(), new Mult(), new Div()
                }
            };

            string assemblyPath = args[0];
            Console.WriteLine("Checking for calculation AddOns in " + assemblyPath);

            Assembly toReflect = Assembly.LoadFrom(assemblyPath);
            var typeList = toReflect.GetTypes();
            Type parentType = typeof(IOperation);
            foreach (Type childType in typeList)
            {
                if (parentType.IsAssignableFrom(childType))
                {            
                    IOperation op = (IOperation)Activator.CreateInstance(childType);
                    calc.operations.Add(op);
                }
            }

            String s;
            Console.WriteLine("Welcome to the Calculator");
            while (true)
            {
                Console.Write("Enter Command:");
                s = Console.ReadLine();
                if (s == "exit")
                    break;
                Console.WriteLine(calc.Calculate(s));
            }

        }
    }
}

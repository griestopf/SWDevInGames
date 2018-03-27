using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X01
{
    class Program
    {

        struct Pair
        {
            public int one;
            public int two;
        }

        class Cat
        {
            public string name;
            public int age;
        }


        class A
        {
            public virtual void DoSomething()
            {
                Console.WriteLine("A.DoSomething()");

            }
        }

        class B : A
        {
            public override void DoSomething()
            {
                Console.WriteLine("B.DoSomething()");
            }
        }


        static void Main(string[] args)
        {
            float a = 2;
            Pair p = new Pair { one = 47, two = 11 };
            Cat c = new Cat { name = "Whiskers", age = 5 };

            Pair q = p;
            Cat d = c;

            q.one = 99;
            Console.WriteLine("p.one: " + p.one);

            d.age = 10;
            Console.WriteLine("c.age: " + c.age);

            Cat[] lotsaCats = new Cat[]
            {
                new Cat {age = 2, name= "Garfield" },
                new Cat {age = 5, name= "Mausetod" },
                new Cat {age = 7, name= "Fritz" },
            };

            Pair[] lotsaPairs = new Pair[]
            {
                new Pair {one = 2, two= 22 },
                new Pair {one = 5, two= 33 },
                new Pair {one = 7, two= 44 },
            };



            A anA = new B();

            anA.DoSomething();

            Console.ReadKey();

        }
    }
}

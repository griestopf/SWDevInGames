using System;

namespace ContainerExample
{

    public class MyContainer
    {
        private int[] _theObjects;
        private int _n;

        public MyContainer()
        {
            _theObjects = new int[4];
            _n = 0;
        }

        public void Add(int i)
        {
            _theObjects[_n++] = i;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            MyContainer myContainer = new MyContainer();

            myContainer.Add(2);
            myContainer.Add(4711);
            myContainer.Add(123);
            myContainer.Add(456);

        }
    }
}

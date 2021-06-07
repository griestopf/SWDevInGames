using System;

using CalcApp;


namespace CalcAddOn
{
    public class Modulus : IOperation
    {
        public char Symbol => '%';
        public int Operate(int l, int r)
        {
            return r % l;
        }
    }

    public class LeftShift : IOperation
    {
        public char Symbol => '<';
        public int Operate(int l, int r)
        {
            return r << l;
        }
    }

    public class RightShift : IOperation
    {
        public char Symbol => '>';
        public int Operate(int l, int r)
        {
            return r >> l;
        }
    }
}

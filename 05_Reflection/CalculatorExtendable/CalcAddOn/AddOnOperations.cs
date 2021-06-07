using System;

using CalcApp;


namespace CalcAddOn
{
    public class Modulus : IOperation
    {
        public char Symbol => '%';
        public int Operate(int l, int r)
        {
            return l % r;
        }
    }

    public class LeftShift : IOperation
    {
        public char Symbol => '<';
        public int Operate(int l, int r)
        {
            return l << r;
        }
    }

    public class RightShift : IOperation
    {
        public char Symbol => '>';
        public int Operate(int l, int r)
        {
            return l >> r;
        }
    }
}

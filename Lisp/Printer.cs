using System;
using System.Collections.Generic;
using System.Text;

namespace Lisp
{
    public class Printer
    {
        public static string Print(AstObject ao)
        {
            switch (ao)
            {
                case AstAtom aa:
                    return PrintAtom(aa);
                case AstList al:
                    return PrintList(al);
                default:
                    throw new Exception("Dafuq?");
            }
        }

        private static string PrintList(AstList al)
        {
            var ret = new StringBuilder();
            ret.Append('(');
            var list = new Queue<AstObject>(al.Objects);
            while (list.Count > 0)
            {
                ret.Append(Print(list.Dequeue()));
                if (list.Count != 0)
                    ret.Append(' ');
            }

            ret.Append(')');
            return ret.ToString();
        }

        private static string PrintAtom(AstAtom ao)
        {
            return ao.Represent();
        }
    }
}
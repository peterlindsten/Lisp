using System;
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
            foreach (var ao in al.Objects)
            {
                ret.Append(Print(ao));
                ret.Append(' ');
            }

            ret.Remove(ret.Length - 1, 1);

            ret.Append(')');
            return ret.ToString();
        }

        private static string PrintAtom(AstAtom ao)
        {
            return ao.Text;
        }
    }
}
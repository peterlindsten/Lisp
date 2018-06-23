using System;

namespace Lisp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Ready for hacking!");
            while (true)
            {
                Console.Write("user> ");
                var line = Console.ReadLine();
                if (line == null)
                {
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                Console.WriteLine(Rep(line));
            }
        }

        public static string Rep(string line)
        {
            return Print(Eval(Read(line)));
        }

        public static AstObject Read(string expression)
        {
            return Reader.Read(expression);
        }

        public static AstObject Eval(AstObject ao)
        {
            return ao;
        }

        public static string Print(AstObject ao)
        {
            return Printer.Print(ao);
        }
    }

}
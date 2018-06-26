using System;

namespace Lisp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var env = Environment.StandardEnv();
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

                try
                {
                    Console.WriteLine(Rep(line, env));
                }
                catch (EvaluationException ee)
                {
                    Console.WriteLine(ee.Message);
                }
            }
        }

        public static string Rep(string line, Environment env)
        {
            return Print(Eval(Read(line), env));
        }

        public static AstObject Read(string expression)
        {
            return Reader.Read(expression);
        }

        public static AstObject Eval(AstObject ao, Environment env)
        {
            return Evaluator.Eval(ao, env);
        }

        public static string Print(AstObject ao)
        {
            return Printer.Print(ao);
        }
    }

}
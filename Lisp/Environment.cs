using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisp
{
    public class Environment : Dictionary<AstSymbol, AstObject>
    {
        public Environment(Environment env) : base(env)
        {
        }

        public Environment()
        {
        }

        public static Environment StandardEnv()
        {
            var ret = new Environment
            {
                {new AstSymbol("+"), new AstFunc(l => l.Sum(ao => (AstNumber) ao))},
                {new AstSymbol("*"), new AstFunc(l => l.Multiply(ao => (AstNumber) ao))},
                {new AstSymbol("-"), new AstFunc(l => l.Subtract(ao => (AstNumber) ao))},
                {new AstSymbol("/"), new AstFunc(l => l.Divide(ao => (AstNumber) ao))},
                {new AstSymbol("list"), new AstFunc(l => new AstList(l))},
                {new AstSymbol(">"), new AstFunc(l => l.GreaterThen(ao => (AstNumber) ao))},
                {new AstSymbol("<"), new AstFunc(l => l.LessThen(ao => (AstNumber) ao))},
                {new AstSymbol("first"), new AstFunc(l => ((AstList) l.First()).Objects.First())},
                {new AstSymbol("rest"), new AstFunc(l => new AstList(((AstList) l.First()).Objects.Skip(1)))},
                {
                    new AstSymbol("print"), new AstFunc(l =>
                    {
                        Console.WriteLine(l.Aggregate("", (s, a) => s + a.ToString())); // Represent?
                        return new AstBoolean(true);
                    })
                },
            };
            return ret;
        }
    }
}
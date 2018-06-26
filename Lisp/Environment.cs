using System.Collections.Generic;

namespace Lisp
{
    public class Environment : Dictionary<AstSymbol, AstObject>
    {
        public static Environment StandardEnv()
        {
            var ret = new Environment
            {
                {new AstSymbol("+"), new AstFunc(l => l.Sum(ao => (AstNumber) ao))},
                {new AstSymbol("*"), new AstFunc(l => l.Multiply(ao => (AstNumber) ao))},
                {new AstSymbol("-"), new AstFunc(l => l.Subtract(ao => (AstNumber) ao))},
                {new AstSymbol("/"), new AstFunc(l => l.Divide(ao => (AstNumber) ao))},
                {new AstSymbol("list"), new AstFunc(l => new AstList(l))},
                {new AstSymbol(">"), new AstFunc(l => l.LargerThen(ao => (AstNumber) ao))},
            };
            return ret;
        }
    }
}
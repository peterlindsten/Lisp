using System;
using System.Diagnostics;
using System.Linq;

namespace Lisp
{
    public class Evaluator
    {
        public static AstObject Eval(AstObject ao, Environment env)
        {
            if (ao is AstSymbol a)
            {
                if (env.ContainsKey(a))
                    return env[a];
                throw new EvaluationException($"Symbol {a.Represent()} not found");
            }

            if (ao is AstNumber an)
                return an;
            if (ao is AstBoolean ab)
                return ab;
            if (ao is AstList al) // TODO: Empty list
            {
                if (al.Objects[0].Equals(new AstSymbol("define")))
                {
                    if (al.Objects.Count == 3 && al.Objects[1] is AstSymbol symbol)
                    {
                        env[symbol] = al.Objects[2]; // TODO: This sets ref, not copying value. "Lazy" Right or wrong?
                        return symbol;
                    }
                }

                if (al.Objects[0].Equals(new AstSymbol("if")))
                {
                    if (al.Objects.Count >= 3 && al.Objects.Count <= 4)
                    {
                        var cond = Eval(al.Objects[1], env);
                        if (new AstBoolean(false) != cond)
                        {
                            return Eval(al.Objects[2], env);
                        }

                        if (al.Objects.Count == 4 && new AstBoolean(false) == cond)
                        {
                            return Eval(al.Objects[3], env);
                        }

                        return cond;
                    }
                }
                var f = (AstFunc) Eval(al.Objects[0], env);
                var args = al.Objects.Skip(1).Select(o => Eval(o, env));
                try
                {
                    return f.f.Invoke(args.ToList());
                }
                catch (InvalidCastException e)
                {
                    throw new EvaluationException(e.Message);
                }
            }

            Debug.Assert(false, "Encountered unknown AstObject");
            throw new EvaluationException($"Encountered unknown AstObject: {ao}");
        }
    }

    public class EvaluationException : Exception
    {
        public EvaluationException(string s) : base(s)
        {
        }
    }
}
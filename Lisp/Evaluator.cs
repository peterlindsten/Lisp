using System;
using System.Collections.Generic;
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
                        env[symbol] =
                            Eval(al.Objects[2],
                                env); // TODO: Eager, right or wrong? Lazy did implicit quote - Need later eval for that
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

                if (al.Objects[0].Equals(new AstSymbol("lambda")))
                {
                    if (al.Objects.Count == 3 && al.Objects[1] is AstList list)
                    {
                        AstObject Func(List<AstObject> x)
                        {
                            var environment = new Environment(env);
                            for (var index = 0; index < list.Objects.Count; index++)
                            {
                                var argName = list.Objects[index];
                                if (argName is AstSymbol @as)
                                {
                                    if (x.Count > index)
                                        environment[@as] = x[index];
                                    else
                                        throw new EvaluationException("Currying not yet supported");
//                                        return
//                                            Eval(new AstList(new AstSymbol("lambda"),
//                                                    new AstList(list.Objects.Take(index + 1).ToList()),
//                                                    al.Objects[2]),
//                                                environment); 
                                }
                                else
                                {
                                    throw new EvaluationException(
                                        "Encountered non-symbol in parameter definition list: " + argName.GetType());
                                }
                            }

                            return Eval(al.Objects[2], environment);
                        }

                        return new AstFunc(Func);
                    }
                }

                var f = (AstFunc) Eval(al.Objects[0], env);
                var args = al.Objects.Skip(1).Select(o => Eval(o, env));
                try
                {
                    return f.F.Invoke(args.ToList());
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
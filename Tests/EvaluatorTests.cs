using Lisp;
using Xunit;
using static Lisp.Evaluator;

namespace Tests
{
    public class EvaluatorTests
    {
        private Environment e = new Environment()
        {
            {new AstSymbol("one"), new AstNumber(1)},
            {new AstSymbol("+"), new AstFunc(l => l.Sum(ao => (AstNumber) ao))},
        };

        [Fact]
        public void SymbolIsLookedup() =>
            Assert.Equal(new AstNumber(1), Eval(new AstSymbol("one"), e));

        [Fact]
        public void MissingSymbolThrows() =>
            Assert.Throws<EvaluationException>(() => Eval(new AstSymbol("two"), e));

        [Fact]
        public void NumberIsReturned() =>
            Assert.Equal(new AstNumber(1), Eval(new AstNumber(1), e));

        [Fact]
        public void FuncIsExecuted() =>
            Assert.Equal(new AstNumber(2),
                Eval(new AstList(new AstSymbol("+"), new AstNumber(1), new AstNumber(1)),
                    Environment.StandardEnv()));

        [Fact]
        public void DefineReturnsSymbol() =>
            Assert.Equal(new AstSymbol("two"),
                Eval(new AstList(new AstSymbol("define"), new AstSymbol("two"), new AstNumber(2)), e));

        [Fact]
        public void DefinedSymbolCanBeLookedup()
        {
            Eval(new AstList(new AstSymbol("define"), new AstSymbol("two"), new AstNumber(2)), e);
            Assert.Equal(new AstNumber(2), Eval(new AstSymbol("two"), e));
        }

        [Fact]
        public void DefineTakesExactly2Arguments()
        {
            Assert.Throws<EvaluationException>(() =>
                Eval(new AstList(new AstSymbol("define")), e));
            Assert.Throws<EvaluationException>(() =>
                Eval(new AstList(new AstSymbol("define"),
                    new AstSymbol("define")), e));
            Assert.Throws<EvaluationException>(() =>
                Eval(
                    new AstList(new AstSymbol("define"),
                        new AstSymbol("define"),
                        new AstSymbol("define"),
                        new AstSymbol("define")), e));
        }

        [Fact]
        public void DefineFirstArgHasToBeSymbol() =>
            Assert.Throws<EvaluationException>(() =>
                Eval(
                    new AstList(
                        new AstSymbol("define"),
                        new AstList(),
                        new AstSymbol("a")),
                    e));

        [Fact]
        public void IfTrueEvalsThenButNotElse()
        {
            Assert.Throws<EvaluationException>(() => Eval(new AstSymbol("two"), e));
            Assert.Equal(new AstNumber(1),
                Eval(new AstList(new AstSymbol("if"),
                        new AstBoolean(true),
                        new AstSymbol("one"),
                        new AstSymbol("two")),
                    e));
        }


        [Fact]
        public void IfFalseEvalsElseButNotThen()
        {
            Assert.Throws<EvaluationException>(() => Eval(new AstSymbol("two"), e));
            Assert.Equal(new AstNumber(1),
                Eval(new AstList(new AstSymbol("if"),
                        new AstBoolean(false),
                        new AstSymbol("two"),
                        new AstSymbol("one")),
                    e));
        }

        [Fact]
        public void IfTakes2Or3Arguments()
        {
            Eval(new AstList(new AstSymbol("if"),
                    new AstBoolean(false),
                    new AstSymbol("two"),
                    new AstSymbol("one")),
                e);
            Eval(new AstList(new AstSymbol("if"),
                    new AstBoolean(true),
                    new AstSymbol("one")),
                e);
            Assert.Throws<EvaluationException>(
                () => Eval(new AstList(new AstSymbol("if"),
                        new AstBoolean(true)),
                    e));
            Assert.Throws<EvaluationException>(
                () => Eval(new AstList(new AstSymbol("if"),
                        new AstBoolean(true),
                        new AstBoolean(true),
                        new AstBoolean(true),
                        new AstBoolean(true)),
                    e));
        }

        [Fact]
        public void LambdaReturnsAppliable()
        {
            // Applied identity: ((lambda (a) a) 1) => 1
            Assert.Equal(new AstNumber(1), Eval(new AstList(new AstList(new AstSymbol("lambda"),
                new AstList(new AstSymbol("a")),
                new AstSymbol("a")), new AstNumber(1)), e));
        }

        [Fact]
        public void LambdaParamsAreBound()
        {
            // ((lambda (a) (+ 1 a) 1) => 2
            Assert.Equal(new AstNumber(2),
                Eval(new AstList(new AstList(new AstSymbol("lambda"),
                            new AstList(new AstSymbol("a")),
                            new AstList(new AstSymbol("+"),
                                new AstSymbol("a"),
                                new AstNumber(1))),
                        new AstNumber(1)),
                    e));
        }
    }
}
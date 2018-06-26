using System;
using System.Collections.Generic;
using Lisp;
using Xunit;
using static Lisp.Reader;

namespace Tests
{
    public class ReaderTests
    {
        private static readonly Queue<Token> Empty = new Queue<Token>();
        private readonly Queue<Token> expect = new Queue<Token>();
        private readonly Queue<Token> input;

        public ReaderTests()
        {
            input = expect;
        }

        [Fact]
        public void LexEmptyReturnsEmpty() =>
            Assert.Equal(Empty, Lex(""));

        [Fact]
        public void LexSpaceReturnsEmpty() =>
            Assert.Equal(Empty, Lex(" "));

        [Fact]
        public void LexTextReturnsText()
        {
            expect.Enqueue(new Token("Text"));
            Assert.Equal(expect, Lex("Text"));
        }

        [Fact]
        public void LexParensAreSplit()
        {
            expect.Enqueue(new Token("("));
            expect.Enqueue(new Token(")"));
            Assert.Equal(expect, Lex("()"));
        }

        [Fact]
        public void LexListsSplitsOnSpace()
        {
            expect.Enqueue(new Token("("));
            expect.Enqueue(new Token("ra"));
            expect.Enqueue(new Token("wr"));
            expect.Enqueue(new Token(")"));
            Assert.Equal(expect, Lex("(ra wr)"));
        }

        [Fact]
        public void ParseEmptyThrows() =>
            Assert.Throws<InvalidOperationException>(() => Parse(Empty));

        [Fact]
        public void ParseUnmatchedListThrows()
        {
            input.Enqueue(new Token("("));
            Assert.Throws<Exception>(() => Parse(input));
        }

        [Fact]
        public void ParseList()
        {
            input.EnqueueAll(new Token("("),
                new Token("ra"),
                new Token("("),
                new Token("wr"),
                new Token(")"),
                new Token(")"));
            var expected = new AstList(new AstSymbol("ra"), new AstList(new AstSymbol("wr")));
            Assert.Equal(expected, Parse(input));
        }
        
        [Fact]
        public void ParseInteger()
        {
            input.Enqueue(new Token("1"));
            Assert.Equal(new AstNumber(1), Parse(input));
        }
    }

}
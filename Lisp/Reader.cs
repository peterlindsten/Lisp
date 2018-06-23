﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lisp
{
    public static class Reader
    {
        public static AstObject Read(string s)
        {
            return Parse(Lex(s));
        }

        public static Queue<Token> Lex(string s)
        {
            var ret = new Queue<Token>();
            var buf = new StringBuilder();
            foreach (var c in s)
            {
                switch (c)
                {
                    case ' ':
                        HandleBuffer(buf, ret);
                        break;
                    case '(':
                        ret.Enqueue(new Token(c.ToString()));
                        break;
                    case ')':
                        HandleBuffer(buf, ret);
                        ret.Enqueue(new Token(c.ToString()));
                        break;
                    default:
                        buf.Append(c);
                        break;
                }
            }
            HandleBuffer(buf, ret);

            return ret;
        }

        private static void HandleBuffer(StringBuilder buf, Queue<Token> ret)
        {
            if (buf.Length > 0)
            {
                ret.Enqueue(new Token(buf.ToString()));
                buf.Clear();
            }
        }

        public static AstObject Parse(Queue<Token> l)
        {
            var t = l.Peek();

            if (t.Text.Equals("("))
            {
                return ParseList(l);
            }
            else
            {
                return ParseAtom(l);
            }
        }

        private static AstAtom ParseAtom(Queue<Token> tokens)
        {
            return new AstAtom(tokens.Dequeue().Text);
        }

        private static AstList ParseList(Queue<Token> tokens)
        {
            var ret = new AstList();
            tokens.Dequeue();
            while (tokens.Count > 0)
            {
                if (tokens.Peek().Text.Equals(")"))
                {
                    tokens.Dequeue();
                    return ret;
                }

                ret.Add(Parse(tokens));
            }

            throw new Exception("Couldn't find end of list");
        }
    }
}
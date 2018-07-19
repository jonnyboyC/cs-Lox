using System;
using System.Collections.Generic;
using csLox.Scanning;

namespace csLox.Parsing
{
    internal abstract class Expr { }

    internal class Binary : Expr
    {
        public Expr Left { get; }
        public Token OpCode { get; }
        public Expr Right { get; }
        Binary(Expr left, Token opCode, Expr right)
        {
            Left = left;
            OpCode = opCode;
            Right = right;
        }
    }

    internal class Grouping : Expr
    {
        public Expr Expression { get; }
        Grouping(Expr expression)
        {
            Expression = expression;
        }
    }

    internal class Literal : Expr
    {
        public Object Value { get; }
        Literal(Object value)
        {
            Value = value;
        }
    }

    internal class Unary : Expr
    {
        public Token OpCode { get; }
        public Expr Right { get; }
        Unary(Token opCode, Expr right)
        {
            OpCode = opCode;
            Right = right;
        }
    }
}

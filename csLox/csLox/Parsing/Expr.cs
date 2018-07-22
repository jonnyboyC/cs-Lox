using System;
using System.Collections.Generic;
using csLox.Scanning;

namespace csLox.Parsing
{
    internal abstract class Expr
    {
        internal abstract T Accept<T>(Visitor<T> visitor);

        internal interface Visitor<T> 
        {
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitConditionalExpr(Conditional expr);
            T VisitBinaryExpr(Binary expr);
            T VisitUnaryExpr(Unary expr);
        }


        internal class Grouping : Expr
        {
            internal Expr Expression { get; }
            internal Grouping(Expr expression)
            {
                Expression = expression;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        internal class Literal : Expr
        {
            internal Object Value { get; }
            internal Literal(Object value)
            {
                Value = value;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        internal class Conditional : Expr
        {
            internal Expr Condition { get; }
            internal Expr TrueExpr { get; }
            internal Expr FalseExpr { get; }
            internal Conditional(Expr condition, Expr trueExpr, Expr falseExpr)
            {
                Condition = condition;
                TrueExpr = trueExpr;
                FalseExpr = falseExpr;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitConditionalExpr(this);
            }
        }

        internal class Binary : Expr
        {
            internal Expr Left { get; }
            internal Token OpCode { get; }
            internal Expr Right { get; }
            internal Binary(Expr left, Token opCode, Expr right)
            {
                Left = left;
                OpCode = opCode;
                Right = right;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        internal class Unary : Expr
        {
            internal Token OpCode { get; }
            internal Expr Right { get; }
            internal Unary(Token opCode, Expr right)
            {
                OpCode = opCode;
                Right = right;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }
    }
}

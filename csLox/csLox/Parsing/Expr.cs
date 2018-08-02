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
            T VisitAssignExpr(Assign expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLambdaExpr(Lambda expr);
            T VisitLogicalExpr(Logical expr);
            T VisitLiteralExpr(Literal expr);
            T VisitConditionalExpr(Conditional expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGetExpr(Get expr);
            T VisitSetExpr(Set expr);
            T VisitThisExpr(This expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }


        internal class Assign : Expr
        {
            internal Token Name { get; }
            internal Expr Value { get; }
            internal Assign(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
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

        internal class Lambda : Expr
        {
            internal List<Token> Parameter { get; }
            internal List<Stmt> Body { get; }
            internal Lambda(List<Token> parameter, List<Stmt> body)
            {
                Parameter = parameter;
                Body = body;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLambdaExpr(this);
            }
        }

        internal class Logical : Expr
        {
            internal Expr Left { get; }
            internal Token OpCode { get; }
            internal Expr Right { get; }
            internal Logical(Expr left, Token opCode, Expr right)
            {
                Left = left;
                OpCode = opCode;
                Right = right;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
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

        internal class Call : Expr
        {
            internal Expr Callee { get; }
            internal Token Paren { get; }
            internal List<Expr> Arguments { get; }
            internal Call(Expr callee, Token paren, List<Expr> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        internal class Get : Expr
        {
            internal Expr Instance { get; }
            internal Token Name { get; }
            internal Get(Expr instance, Token name)
            {
                Instance = instance;
                Name = name;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        internal class Set : Expr
        {
            internal Expr Instance { get; }
            internal Token Name { get; }
            internal Expr Value { get; }
            internal Set(Expr instance, Token name, Expr value)
            {
                Instance = instance;
                Name = name;
                Value = value;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }

        internal class This : Expr
        {
            internal Token Keyword { get; }
            internal This(Token keyword)
            {
                Keyword = keyword;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitThisExpr(this);
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

        internal class Variable : Expr
        {
            internal Token Name { get; }
            internal Variable(Token name)
            {
                Name = name;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
    }
}

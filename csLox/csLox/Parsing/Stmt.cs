using System;
using System.Collections.Generic;
using csLox.Scanning;
using Optional;

namespace csLox.Parsing
{
    internal abstract class Stmt
    {
        internal abstract T Accept<T>(Visitor<T> visitor);

        internal interface Visitor<T> 
        {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmtStmt(ExpressionStmt stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
        }


        internal class Block : Stmt
        {
            internal List<Stmt> Statements { get; }
            internal Block(List<Stmt> statements)
            {
                Statements = statements;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        internal class ExpressionStmt : Stmt
        {
            internal Expr Expression { get; }
            internal ExpressionStmt(Expr expression)
            {
                Expression = expression;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpressionStmtStmt(this);
            }
        }

        internal class If : Stmt
        {
            internal Expr Condition { get; }
            internal Stmt ThenBranch { get; }
            internal Option<Stmt> ElseBranch { get; }
            internal If(Expr condition, Stmt thenBranch, Option<Stmt> elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        internal class Print : Stmt
        {
            internal Expr Expression { get; }
            internal Print(Expr expression)
            {
                Expression = expression;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        internal class Var : Stmt
        {
            internal Token Name { get; }
            internal Option<Expr> Initializer { get; }
            internal Var(Token name, Option<Expr> initializer)
            {
                Name = name;
                Initializer = initializer;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        internal class While : Stmt
        {
            internal Expr Condition { get; }
            internal Stmt Body { get; }
            internal While(Expr condition, Stmt body)
            {
                Condition = condition;
                Body = body;
            }
            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }
    }
}

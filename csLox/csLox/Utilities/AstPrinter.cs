using System;
using System.Collections.Generic;
using System.Text;
using csLox.Parsing;
using csLox.Linq;
using csLox.Scanning;

namespace csLox.Utilities
{
    internal class AstPrinter : Expr.Visitor<String>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitConditionalExpr(Expr.Conditional expr)
        {
            return Parenthesize("Conditional", expr.Condition, expr.TrueExpr, expr.FalseExpr);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.OpCode.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("Group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.OpCode.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] expressions)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in expressions)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }

        public static void Main(string[] args)
        {
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.Minus, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.Star, "*", null, 1),
                    new Expr.Grouping(
                        new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));
            Console.Read();
        }
    }
}

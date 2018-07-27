using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csLox.Parsing;
using csLox.Scanning;

namespace csLox.Utilities
{
    internal class AstPrinter : Expr.Visitor<string>, Stmt.Visitor<IEnumerable<string>>
    {
        public string PrintExpr(Expr expr)
        {
            return expr.Accept(this);
        }

        public IEnumerable<string> PrintStmt(Stmt stmt)
        {
            return stmt.Accept(this);
        }

        public string VisitConditionalExpr(Expr.Conditional expr)
        {
            return Parenthesize("?:", expr.Condition, expr.TrueExpr, expr.FalseExpr);
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

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.OpCode.Lexeme, expr.Left, expr.Right);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return $"{expr.Name.Lexeme} = {PrintExpr(expr.Value)}";
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.Name.Lexeme;
        }

        public IEnumerable<string> VisitBlockStmt(Stmt.Block stmt)
        {
            StringBuilder builder = new StringBuilder();

            yield return "{";
            foreach (Stmt subStmt in stmt.Statements)
            {
                foreach (string print in PrintStmt(subStmt))
                {
                    yield return $"  {print}";
                }
            }
            yield return "}";
        }

        public IEnumerable<string> VisitExpressionStmtStmt(Stmt.ExpressionStmt stmt)
        {
            yield return $"{PrintExpr(stmt.Expression)};";
        }

        public IEnumerable<string> VisitIfStmt(Stmt.If stmt)
        {
            string[] ifBody = PrintStmt(stmt.ThenBranch).ToArray();
            yield return $"if ({PrintExpr(stmt.Condition)}) {ifBody[0]}";

            foreach (string print in ifBody.Skip(1))
            {
                yield return print;
            }

            string[] elseBody = stmt.ElseBranch.Match(
                some: elseStmt => PrintStmt(elseStmt).ToArray(),
                none: () => new string[0]
            );

            if (elseBody.Any())
            {
                yield return $"else {elseBody[0]}";
                foreach (string print in elseBody.Skip(1))
                {
                    yield return print;
                }
            }
        }

        public IEnumerable<string> VisitPrintStmt(Stmt.Print stmt)
        {
            yield return $"print {PrintExpr(stmt.Expression)};";
        }

        public IEnumerable<string> VisitVarStmt(Stmt.Var stmt)
        {
            yield return stmt.Initializer.Match(
                some: init => $"var {stmt.Name} = {PrintExpr(init)}",
                none: () => $"var {stmt.Name};"
            );
        }

        public IEnumerable<string> VisitWhileStmt(Stmt.While stmt)
        {
            string[] whileBody = PrintStmt(stmt.Body).ToArray();
            yield return $"if ({PrintExpr(stmt.Condition)}) {whileBody[0]}";

            foreach (string print in whileBody.Skip(1))
            {
                yield return print;
            }
        }

        public IEnumerable<string> VisitBreakStmt(Stmt.Break stmt)
        {
            yield return "break;";
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            string arguments = string.Join(", ", expr.Arguments.Select(e => PrintExpr(e)));

            return $"{PrintExpr(expr.Callee)}({arguments})";
        }

        public IEnumerable<string> VisitFunctionStmt(Stmt.Function stmt)
        {
            string parameters = string.Join(", ", stmt.Parameter.Select(p => p.Lexeme));
            string[] body = stmt.Body.SelectMany(s => PrintStmt(s)).ToArray();
            
            yield return $"fun {stmt.Name}({parameters}) {{";

            foreach (string bodyStmt in body)
            {
                yield return $"  {bodyStmt}";
            }
            yield return "}";
        }

        public IEnumerable<string> VisitReturnStmt(Stmt.Return stmt)
        {
            yield return stmt.Value.Match(
                some: value => $"return {value};",
                none: () => "return;"
            );
        }

        public string VisitLambdaExpr(Expr.Lambda expr)
        {
            string parameters = string.Join(", ", expr.Parameter.Select(p => p.Lexeme));
            string body = string.Join(" ", expr.Body.SelectMany(s => PrintStmt(s)));

            return $"fun ({parameters}) {{ {body} }}";
        }

        public IEnumerable<string> VisitClassStmt(Stmt.Class stmt)
        {
            yield return $"class {stmt.Name} {{";

            foreach (var method in stmt.Methods)
            {
                foreach (string line in PrintStmt(method))
                {
                    yield return $"  {line}";
                }
            }
            yield return "}";
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }
    }
}

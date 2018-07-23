using System;
using System.Collections.Generic;
using System.Linq;
using csLox.Parsing;
using csLox.Scanning;
using Environment = csLox.Scoping.Environment;

namespace csLox.Interpreting
{
    internal class Interpreter : Expr.Visitor<object>, Stmt.Visitor<Dummy>
    {
        public Environment Globals { get; }
        private Environment _environment;
        private bool _break;

        internal Interpreter()
        {
            Globals = new Environment();
            _environment = Globals;
            _break = false;

            Globals.Define("clock", new Globals.Clock() as LoxCallable);
        }

        internal void Interpret(IEnumerable<Stmt> statments)
        {
            try
            {
                foreach (Stmt statement in statments) 
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        internal void Execute(Stmt stmt)
        {
            if (_break) return;
            stmt.Accept(this);
        }

        internal void ExecuteBlock(List<Stmt> statments, Environment environment)
        {
            Environment previous = _environment;
            try 
            {
                _environment = environment;

                foreach (Stmt statement in statments)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this._environment = previous;
            }
        }

        public Dummy VisitExpressionStmtStmt(Stmt.ExpressionStmt stmt)
        {
            Evalutate(stmt.Expression);
            return null;
        }

        public Dummy VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evalutate(stmt.Condition)))
            {

                Execute(stmt.ThenBranch);
            }
            else
            {
                stmt.ElseBranch.MatchSome((elseBranch) => Execute(elseBranch));
            }
            
            return null;
        }

        public Dummy VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evalutate(stmt.Condition)))
            {
                Execute(stmt.Body);
                if (_break) break;
            }
            _break = false;

            return null;
        }

        public Dummy VisitPrintStmt(Stmt.Print stmt)
        {
            object value = Evalutate(stmt.Expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public Dummy VisitVarStmt(Stmt.Var stmt)
        {
            stmt.Initializer.Match(
                some: init => _environment.Define(stmt.Name.Lexeme, Evalutate(init)),
                none: () => _environment.Declare(stmt.Name.Lexeme)
            );

            return null;
        }

        public Dummy VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public Dummy VisitBreakStmt(Stmt.Break stmt)
        {
            _break = true;
            return null;
        }

        private object Evalutate(Expr expr)
        {
            return expr.Accept(this);
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evalutate(expr.Value);

            _environment.Assign(expr.Name, value);
            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evalutate(expr.Left);
            object right = Evalutate(expr.Right);

            switch (expr.OpCode.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left < (double)right;
                case TokenType.Minus:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left - (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                case TokenType.Plus:
                    if (left is double leftDouble && right is double rightDouble)
                    {

                        return leftDouble + rightDouble;
                    }
                    if (left is string leftString && right is string rightString)
                    {
                        return leftString + rightString;
                    }
                    throw new RuntimeError(expr.OpCode, "Operands must be two numbers or two strings.");
                case TokenType.Slash:
                    CheckNumberOperands(expr.OpCode, left, right);
                    double denominator = (double)right;
                    if (denominator == 0) throw new RuntimeError(expr.OpCode, "Divid by zero.");
                    return (double)left / denominator;
                case TokenType.Star:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left * (double)right;
            }

            throw new Exception();
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            object callee = Evalutate(expr.Callee);

            List<object> arguments = expr.Arguments
                .Select(e => Evalutate(e))
                .ToList();

            if (callee is LoxCallable function)
            {
                if (arguments.Count != function.Arity())
                {
                    throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} " +
                        $"arguments but got {arguments.Count}.");
                }

                return function.Call(this, arguments);
            }

            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }

        public object VisitConditionalExpr(Expr.Conditional expr)
        {
            object condition = Evalutate(expr.Condition);

            if (IsTruthy(condition))
            {
                return Evalutate(expr.TrueExpr);
            }
            else
            {
                return Evalutate(expr.FalseExpr);
            }
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evalutate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evalutate(expr.Left);

            if (expr.OpCode.Type == TokenType.Or)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evalutate(expr.Right);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evalutate(expr.Right);

            switch (expr.OpCode.Type) 
            {
                case TokenType.Bang:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(expr.OpCode, right);
                    return -(double)right;
                default:
                    throw new RuntimeError(expr.OpCode, "Unrecongnized unary operator");
            }
        }

        private bool IsTruthy(object obj)
        {
            if (obj is null) return false;
            if (obj is bool boolObj) return boolObj;
            return true;
        }

        private void CheckNumberOperand(Token OpCode, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(OpCode, "Operand must be a number");
        }

        private void CheckNumberOperands(Token OpCode, object left, object right) 
        {
            if (left is double && right is double) return;
            throw new RuntimeError(OpCode, "Operands must be numbers");
        }

        private bool TryGetNumberOperands(object leftOperand, object rightOperand, out double left, out double right)
        {
            return TryGetTypeOperands(leftOperand, rightOperand, out left, out right);
        }

        private bool TryGetStringOperands(object leftOperand, object rightOperand, out string left, out string right)
        {
            return TryGetTypeOperands(leftOperand, rightOperand, out left, out right);
        }

        private bool TryGetTypeOperands<T> (object leftOperand, object rightOperand, out T left, out T right)
        {
            left = default(T);
            right = default(T);

            if (leftOperand is T && rightOperand is T) 
            {
                left = (T)leftOperand;
                right = (T)rightOperand;
                return true;
            }

            return false;
        }

        private bool IsEqual(object a, object b) 
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        private string Stringify(object obj) 
        {
            if (obj is null) return "nil";

            if (obj is double loxDouble) {
                string text = loxDouble.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return _environment.Get(expr.Name);
        }
    }
}
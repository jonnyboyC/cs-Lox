using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using csLox.Interpreting;
using csLox.Parsing;
using csLox.Scanning;
using csLox.Linq;

namespace csLox.Resolving
{
    internal class Resolver : Expr.Visitor<LoxVoid>, Stmt.Visitor<LoxVoid>
    {
        private class Scope : Dictionary<string, Variable> { }
        private class Variable
        {
            internal VariableState State { get; set; }
            internal Token Name { get; }

            public Variable(Token name, VariableState state)
            {
                Name = name;
                State = state;
            }
        }


        private readonly Interpreter _interpreter;
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private FunctionType _currentFunction = FunctionType.None;

        private enum FunctionType { None, Function }
        private enum VariableState { Declared, Defined, Used }

        internal Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        internal void Resolve(IEnumerable<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        internal void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveFunction(List<Token> parameters, List<Stmt> body, FunctionType type)
        {
            FunctionType enclosingFunction = _currentFunction;
            _currentFunction = type;

            BeginScope();
            foreach (Token param in parameters)
            {
                Declare(param);
                Define(param);
            }

            Resolve(body);
            EndScope();

            _currentFunction = enclosingFunction;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            foreach (var (scope, i) in _scopes.Select((scope, i) => (scope, i)))
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    // TODO double check this
                    _interpreter.Resolve(expr, i);
                    scope[name.Lexeme] = new Variable(name, VariableState.Used);
                    return;
                }
            }
        }

        private void BeginScope()
        {
            _scopes.Push(new Scope());
        }

        private void EndScope()
        {
            var currentScope = _scopes.Peek();

            foreach (Variable variable in currentScope.Values)
            {
                if (variable.State != VariableState.Used)
                {
                    Lox.Error(variable.Name, $"Local variable {variable.Name.Lexeme} was never used");
                }
            }

            _scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (_scopes.None()) return;

            Scope scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Variable with this name already declared in this scope.");
            }

            scope[name.Lexeme] = new Variable(name, VariableState.Declared);
        }

        private void Define(Token name)
        {
            if (_scopes.None()) return;
            _scopes.Peek()[name.Lexeme] = new Variable(name, VariableState.Defined);
        }

        public LoxVoid VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public LoxVoid VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public LoxVoid VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }

        public LoxVoid VisitBreakStmt(Stmt.Break stmt)
        {
            throw new NotImplementedException();
        }

        public LoxVoid VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (Expr argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public LoxVoid VisitConditionalExpr(Expr.Conditional expr)
        {
            throw new NotImplementedException();
        }

        public LoxVoid VisitExpressionStmtStmt(Stmt.ExpressionStmt stmt)
        {
            Resolve(stmt.Expression);
            return null;
        }

        public LoxVoid VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt.Parameter, stmt.Body, FunctionType.Function);

            return null;
        }

        public LoxVoid VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public LoxVoid VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);

            stmt.ElseBranch.MatchSome(elseBranch => Resolve(elseBranch));
            return null;
        }

        public LoxVoid VisitLambdaExpr(Expr.Lambda lambda)
        {
            ResolveFunction(lambda.Parameter, lambda.Body, FunctionType.Function);

            return null;
        }

        public LoxVoid VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public LoxVoid VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public LoxVoid VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expression);
            return null;
        }

        public LoxVoid VisitReturnStmt(Stmt.Return stmt)
        {
            if (_currentFunction is FunctionType.None)
            {
                Lox.Error(stmt.Keyword, "Cannot return from top-level code.");
            }

            stmt.Value.MatchSome(expr => Resolve(expr));
            return null;
        }

        public LoxVoid VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public LoxVoid VisitVariableExpr(Expr.Variable expr)
        {
            if (_scopes.Any() && _scopes.Peek().TryGetValue(expr.Name.Lexeme, out Variable variable) && variable.State == VariableState.Declared)
            {
               Lox.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public LoxVoid VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            stmt.Initializer.MatchSome(init => Resolve(init));
            Define(stmt.Name);
            return null;
        }

        public LoxVoid VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return null;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using Optional;
using Optional.Unsafe;
using Optional.Linq;
using csLox.Scanning;
using csLox.Exceptions;

namespace csLox.Parsing
{
    internal class Parser 
    {
        private readonly IReadOnlyList<Token> _tokens;
        private int _current = 0;
        private const int MaxParameters = 32;

        internal Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToArray();
        }

        internal IEnumerable<Stmt> Parse() 
        {
            while (!IsAtEnd()) 
            {
                Option<Stmt> declaration = Declaration(false);
                if (declaration.HasValue)
                {
                    yield return declaration.ValueOrFailure();
                }
            }
        }

        private Option<Stmt> Declaration(bool insideLoop)
        {
            try 
            {
                if (Match(TokenType.Class)) return ClassDeclaration().Some();
                if (Match(TokenType.Fun)) return (Function("function") as Stmt).Some();
                if (Match(TokenType.Var)) return VarDeclaration().Some();
                return Statement(insideLoop).Some();
            } catch (ParseError) {
                Synchronize();
                return Option.None<Stmt>();
            }
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.Identifier, "Expected variable name.");

            Option<Expr> initializer = Match(TokenType.Equal)
                ? Expression().Some()
                : Option.None<Expr>();

            Consume(TokenType.SemiColon, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt.Function Function(string kind)
        {
            Token name = Consume(TokenType.Identifier, $"Expect {kind} name.");
            Consume(TokenType.LeftParen, $"Expect '(' after {kind} name.");

            List<Token> parameters;
            List<Stmt> body;
            (parameters, body) = FunctionBody(kind);
            return new Stmt.Function(name, parameters, body);
        }

        private (List<Token> parameters, List<Stmt> body) FunctionBody(string kind)
        {
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (parameters.Count >= MaxParameters)
                    {
                        Error(Peek(), $"Cannot have more than {MaxParameters} parameters.");
                    }

                    parameters.Add(Consume(TokenType.Identifier, "Expect parameter"));
                }
                while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen, "Expect ')' after parameters.");
            Consume(TokenType.LeftBrace, $"Expect '{{' before {kind} body.");
            List<Stmt> body = Block(false).ToList();

            return (parameters, body);
        }

        private Stmt Statement(bool insideLoop)
        {
            if (Match(TokenType.Print)) return PrintStatment();
            if (Match(TokenType.Return)) return ReturnStatement();
            if (Match(TokenType.While)) return WhileStatement();
            if (Match(TokenType.LeftBrace)) return new Stmt.Block(Block(insideLoop).ToList());
            if (Match(TokenType.For)) return ForStatement();
            if (Match(TokenType.If)) return IfStatement(insideLoop);

            if (Match(TokenType.Break))
            {
                if (!insideLoop) throw Error(Previous(), "Break did not occur inside loop");
                return BreakStatement();
            }

            return ExpressionsStatement();
        }

        private Stmt ClassDeclaration()
        {
            Token name = Consume(TokenType.Identifier, "Expect class name.");

            var superclass = Option.None<Expr.Variable>();
            if (Match(TokenType.Less))
            {
                Consume(TokenType.Identifier, "Expect superclass name.");
                superclass = new Expr.Variable(Previous()).Some();
            }

            Consume(TokenType.LeftBrace, "Expect '{' before class body.");

            var methods = new List<Stmt.Function>();
            while(!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                methods.Add(Function("method"));
            }

            Consume(TokenType.RightBrace, "Expect '}' after class body");
            return new Stmt.Class(name, superclass, methods);
        }

        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Option<Expr> value = !Check(TokenType.SemiColon)
                ? Expression().Some()
                : Option.None<Expr>();

            Consume(TokenType.SemiColon, "Expect ';' after return value");
            return new Stmt.Return(keyword, value);
        }

        private Stmt BreakStatement()
        {
            Token keyword = Previous();
            Consume(TokenType.SemiColon, "Expected ';' after break.");

            return new Stmt.Break(keyword);
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LeftParen, "Expect ')' after 'for'.");

            Stmt initializer;
            if (Match(TokenType.SemiColon))
            {
                initializer = null;
            }
            else if (Match(TokenType.Var))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionsStatement();
            }

            Expr condition = null;
            if (!Check(TokenType.SemiColon))
            {
                condition = Expression();
            }
            Consume(TokenType.SemiColon, "Expect ';' after loop condition");

            Expr increment = null;
            if(!Check(TokenType.RightParen))
            {
                increment = Expression();
            }
            Consume(TokenType.RightParen, "Epect ')' after for clauses.");
            Stmt body = Statement(true);

            if (increment != null)
            {
                body = new Stmt.Block(new List<Stmt>()
                {
                    body,
                    new Stmt.ExpressionStmt(increment)
                });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new List<Stmt>() {
                    initializer,
                    body
                });
            }

            return body;
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RightParen, "Expect ')' after condition.None");
            Stmt body = Statement(true);

            return new Stmt.While(condition, body);

        }

        private Stmt IfStatement(bool insideLoop)
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RightParen, "Expect ')' after if ocndition.");

            Stmt thenBranch = Statement(insideLoop);
            Option<Stmt> elseBranch = Match(TokenType.Else)
                ? Statement(insideLoop).Some()
                : Option.None<Stmt>();

            return new Stmt.If(condition, thenBranch, elseBranch); 
        }

        private Expr Expression() 
        {
            return Assignment();
        }

        private Stmt PrintStatment()
        {
            Expr value = Expression();
            Consume(TokenType.SemiColon, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt ExpressionsStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SemiColon, "Expect ';' after expression");
            return new Stmt.ExpressionStmt(expr);
        }

        private IEnumerable<Stmt> Block(bool insideLoop)
        {
            while (!Check(TokenType.RightBrace) && !IsAtEnd()) 
            {
                Option<Stmt> declaration = Declaration(insideLoop);
                if (declaration.HasValue)
                {
                    yield return declaration.ValueOrFailure();
                }
            }

            Consume(TokenType.RightBrace, "Expect '}' after block.");
        }

        private Expr Assignment()
        {
            Expr expr = Conditional();
            
            if (Match(TokenType.Equal)) 
            {
                Token equals = Previous();
                Expr value = Assignment();

                switch (expr)
                {
                    case Expr.Variable variable:
                        Token name = variable.Name;
                        return new Expr.Assign(name, value);
                    case Expr.Get get:
                        return new Expr.Set(get.Instance, get.Name, value);
                    default:
                        Error(equals, "invalid assignment target.");
                        break;
                }
            }

            return expr;
        }

        private Expr Conditional()
        {
            Expr expr = Or();

            if (Match(TokenType.Question))
            {
                Expr trueExpr = Conditional();
                Consume(TokenType.Colon, "Expect ':' after expression");
                Expr falseExpr = Conditional();
                return new Expr.Conditional(expr, trueExpr, falseExpr);
            }

            return expr;
        }

        private Expr Or()
        {
            return LogicalExpr(() => And(), 
                TokenType.Or);
        }

        private Expr And()
        {
            return LogicalExpr(() => Equality(), 
                TokenType.And);
        }

        private Expr Equality() 
        {
            return BinaryExpr(() => Comparison(),
                TokenType.Bang, 
                TokenType.EqualEqual);
        }

        private Expr Comparison()
        {
            return BinaryExpr(() => Addition(), 
                TokenType.Greater, 
                TokenType.GreaterEqual,
                TokenType.Less,
                TokenType.LessEqual);
        }

        private Expr Addition()
        {
            return BinaryExpr(() => Mutiplication(),
                TokenType.Minus,
                TokenType.Plus);
        }

        private Expr Mutiplication()
        {
            return BinaryExpr(() => Unary(),
                TokenType.Slash,
                TokenType.Star);
        }

        private Expr BinaryExpr (Func<Expr> resurse, params TokenType[] types) 
        {
            Expr expr = resurse();

            while (Match(types)) {
                Token opCode = Previous();
                Expr right = resurse();
                expr = new Expr.Binary(expr, opCode, right);
            }

            return expr;
        }

        private Expr LogicalExpr(Func<Expr> resurse, params TokenType[] types)
        {
            Expr expr = resurse();

            if (Match(types))
            {
                Token opCode = Previous();
                Expr right = resurse();
                expr = new Expr.Logical(expr, opCode, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                Token opCode = Previous();
                Expr right = Unary();
                return new Expr.Unary(opCode, right);
            }

            return Call();
        }

        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParen))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(TokenType.Dot))
                {
                    Token name = Consume(TokenType.Identifier, "Expect property name after '.'.");
                    expr = new Expr.Get(expr, name);
                }
                else break;
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (arguments.Count >= MaxParameters)
                    {
                        Error(Peek(), $"Cannot have more than {MaxParameters} arguments.");
                    }
                    arguments.Add(Expression());
                }
                while (Match(TokenType.Comma));
            }

            Token paren = Consume(TokenType.RightParen, "Expect ')' after arguments.");
            return new Expr.Call(callee, paren, arguments);
        }

        private Expr Primary()
        {
            if (Match(TokenType.False)) return new Expr.Literal(false);
            if (Match(TokenType.True)) return new Expr.Literal(true);
            if (Match(TokenType.Nil)) return new Expr.Literal(null);

            if (Match(TokenType.Number, TokenType.String))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.Super))
            {
                Token keyword = Previous();
                Consume(TokenType.Dot, "Expect '.' after 'super'.");
                Token method = Consume(TokenType.Identifier, "Expect super class method name.");
                return new Expr.Super(keyword, method);
            }

            if (Match(TokenType.This))
            {
                return new Expr.This(Previous());
            }

            if (Match(TokenType.Identifier)) 
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LeftParen))
            {
                Expr expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            if (Match(TokenType.Fun))
            {
                return Lambda();
            }

            throw Error(Peek(), "Expected expression.");
        }

        private Expr Lambda()
        {
            Consume(TokenType.LeftParen, $"Expect '(' after fun.");

            List<Token> parameters;
            List<Stmt> body;
            (parameters, body) = FunctionBody("lambda");
            return new Expr.Lambda(parameters, body);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == tokenType;
        }

        private Token Advance() 
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd() 
        {
            return Peek().Type == TokenType.Eof;
        }

        private Token Peek() 
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SemiColon) return;

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                    default:
                        break;
                }

                Advance();
            }
        }
    }
}
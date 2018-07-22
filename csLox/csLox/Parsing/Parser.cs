using System;
using System.Linq;
using System.Collections.Generic;
using csLox.Scanning;
using Optional;

namespace csLox.Parsing
{
    internal class Parser 
    {
        private class ParseError : FormatException {}
        private readonly IReadOnlyList<Token> _tokens;
        private int _current = 0;

        internal Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToArray();
        }

        internal Option<Expr> Parse() 
        {
            try 
            {
                return Expression().Some();
            }
            catch (ParseError)
            {
                return Option.None<Expr>();
            }
        }

        private Expr Expression() 
        {
            return Conditional();
        }

        private Expr Conditional()
        {
            Expr expr = Equality();

            if (Match(TokenType.Question))
            {
                Expr trueExpr = Conditional();
                Consume(TokenType.Colon, "Expect ':' after expression");
                Expr falseExpr = Conditional();
                return new Expr.Conditional(expr, trueExpr, falseExpr);
            }

            return expr;
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

            if (Match(types)) {
                Token opCode = Previous();
                Expr right = resurse();
                expr = new Expr.Binary(expr, opCode, right);
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

            return Primary();
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

            if (Match(TokenType.LeftParen))
            {
                Expr expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected expression.");
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
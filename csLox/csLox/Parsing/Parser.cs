using System;
using System.Collections.Generic;
using csLox.Scanning;

namespace csLox.Parsing
{
    internal class Parser 
    {
        private readonly IReadOnlyList<Token> _tokens;
        private int _current = 0;

        internal Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        private Expr Expression() 
        {
            return Equality();
        }

        private Expr Equality() 
        {
            Expr expr = Comparison();

            while (Match(TokenType.Bang, TokenType.EqualEqual))
            {
                Token opCode = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, opCode, right);
            }

            return expr;
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
    }
}
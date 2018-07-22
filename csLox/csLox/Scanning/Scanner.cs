using Optional;
using System;
using System.Collections.Generic;
using System.Text;
using Optional.Unsafe;

namespace csLox.Scanning
{
    internal class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static readonly IReadOnlyDictionary<string, TokenType> Keywords
            = new Dictionary<string, TokenType>
            {
                {"and",     TokenType.And},
                {"class",   TokenType.Class},
                {"else",    TokenType.Else},
                {"false",   TokenType.False},
                {"for",     TokenType.For},
                {"fun",     TokenType.Fun},
                {"if",      TokenType.If},
                {"nil",     TokenType.Nil},
                {"or",      TokenType.Or},
                {"print",   TokenType.Print},
                {"return",  TokenType.Return},
                {"super",   TokenType.Super},
                {"this",    TokenType.This},
                {"true",    TokenType.True},
                {"var",     TokenType.Var},
                {"while",   TokenType.While}
            };

        public Scanner(string source)
        {
            _source = source;
        }

        public IEnumerable<Token> ScanTokens()
        {
            _line = 0;
            while (!IsAtEnd())
            {
                _start = _current;
                Option<Token> token = ScanToken();
                if (token.HasValue) yield return token.ValueOrFailure();
            }

            yield return new Token(TokenType.Eof, "", null, _line);
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private Option<Token> ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': return GenerateToken(TokenType.LeftParen).Some();
                case ')': return GenerateToken(TokenType.RightParen).Some();
                case '{': return GenerateToken(TokenType.LeftBrace).Some();
                case '}': return GenerateToken(TokenType.RightBrace).Some();
                case ',': return GenerateToken(TokenType.Comma).Some();
                case '.': return GenerateToken(TokenType.Dot).Some();
                case '-': return GenerateToken(TokenType.Minus).Some();
                case '+': return GenerateToken(TokenType.Plus).Some();
                case ';': return GenerateToken(TokenType.SemiColon).Some();
                case '*': return GenerateToken(TokenType.Star).Some();
                case '?': return GenerateToken(TokenType.Question).Some();
                case ':': return GenerateToken(TokenType.Colon).Some();

                // operators
                case '!': return GenerateToken(Match('=') ? TokenType.BangEqual : TokenType.Bang).Some();
                case '=': return GenerateToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal).Some();
                case '<': return GenerateToken(Match('=') ? TokenType.LessEqual : TokenType.Less).Some();
                case '>': return GenerateToken(Match('=') ? TokenType.GreaterEqual : TokenType.Equal).Some();
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else if (Match('*')) 
                    {
                        MultiLine();
                    }
                    else
                    {
                        return GenerateToken(TokenType.Slash).Some();
                    }
                    break;

                // whitespace
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    _line++;
                    break;

                case '"': return String().Some();

                // unrecognized character
                default:
                    if (IsDigit(c))
                    {
                        return Number().Some();
                    } 
                    else if (IsAlpha(c))
                    {
                        return Identifier().Some();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                        break;
                    }
            }

            return Option.None<Token>();
        }

        private Token String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
            }

            Advance();
            string value = _source.Substring(_start + 1, _current - _start - 2);
            return GenerateToken(TokenType.String, value);
        }

        private Token Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            return GenerateToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
        }

        private Token Identifier()
        {
            while (IsALphaNumeric(Peek())) Advance();

            string text = _source.Substring(_start, _current - _start);
            if (Keywords.TryGetValue(text, out TokenType type))
            {
                return GenerateToken(type);
            }
            else
            {
                return GenerateToken(TokenType.Identifier);
            }
        }

        private void MultiLine()
        {
            bool star = false;

            while (Peek() != '/' && !star && !IsAtEnd())
            {
                char c = Peek();
                if (c == '\n') _line++;
                star = c == '*';
                Advance();
            }
            Advance();
        }

        private Token GenerateToken(TokenType type)
        {
            return GenerateToken(type, null);
        }

        private Token GenerateToken(TokenType type, object literal)
        {
            string text = _source.Substring(_start, _current - _start);
            return new Token(type, text, literal, _line);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c == '_');
        }

        private bool IsALphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}

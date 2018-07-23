using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Scanning
{
    internal enum TokenType
    {
        // Single-character tokens
        LeftParen, RightParen, LeftBrace, RightBrace,
        Comma, Dot, Minus, Plus, SemiColon, Slash, Star,
        Question, Colon,

        // One or two character tokens.
        Bang, BangEqual,
        Equal, EqualEqual,
        Greater, GreaterEqual,
        Less, LessEqual,

        // Literals
        Identifier, String, Number,

        // Keywords
        And, Break, Class, Continue,Else, False, Fun,
        For, If, Nil, Or, Print, Return, Super, This,
        True, Var, While,

        Eof
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    internal enum TokenType
    {
        // Single-character tokens
        Left_Paren, Right_Paren, Left_Brace, Right_Brace,
        Comma, Dot, Minus, Plus, SemiColon, Slash, Star,

        // One or two character tokens.
        Bang, BangEqual,
        Equal, EqualEqual,
        Greater, GreaterEqual,
        Less, LessEqual,

        // Literals
        Identifier, String, Number,

        // Keywords
        And, Class, Else, False, Fun, For, If, Nil, Or,
        Print, Return, Super, This, True, Var, While,

        Eof
    }
}

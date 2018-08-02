using csLox.Exceptions;
using csLox.Scanning;
using Optional;
using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Interpreting
{
    internal class LoxInstance
    {
        private readonly LoxClass _class;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass @class)
        {
            _class = @class;
        }

        public void Set(Token name, object value)
        {
            _fields[name.Lexeme] = value;
        }

        public object Get(Token name)
        {
            if (_fields.TryGetValue(name.Lexeme, out object value))
            {
                return value;
            }

            Option<LoxFunction> methodOption = _class.FindMethod(this, name.Lexeme);
            return methodOption.Match(
                some: method => method,
                none: () => throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.")
            );
        }

        public override string ToString()
        {
            return $"{_class.Name} instance";
        }
    }
}

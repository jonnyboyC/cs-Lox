using System.Collections.Generic;
using Optional;
using csLox.Exceptions;
using csLox.Scanning;

namespace csLox.Scoping
{
    internal class Environment
    {
        private static readonly object unitialized = new object();
        private IDictionary<string, object> values 
            = new Dictionary<string, object>();

        private Option<Environment> _enclosing = Option.None<Environment>();

        internal Environment()
        {
            _enclosing = Option.None<Environment>();
        }

        internal Environment (Environment enclosing)
        {
            _enclosing = enclosing.Some();
        }

        internal void Declare(string name)
        {
            values[name] = unitialized;
        }
    
        internal void Define(string name, object value)
        {
            values[name] = value;
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            _enclosing.Match(
                some: (parentScope) => parentScope.Assign(name, value),
                none: () => throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.")
            );
        }

        internal object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out var value))
            {
                if (value == unitialized) throw new RuntimeError(name, $"Attempted to use unitialized variable {name.Lexeme}.");
                return value;
            }

            return _enclosing.Match(
                some: (parentScope) => parentScope.Get(name),
                none: () => throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}.'")
            );
        }
    }
}
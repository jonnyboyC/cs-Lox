using System;
using System.Collections.Generic;
using csLox.Parsing;
using Environment = csLox.Scoping.Environment;
using Optional;

namespace csLox.Interpreting
{
    internal class LoxFunction : LoxCallable
    {
        private readonly Stmt.Function _declaration;
        LoxFunction(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        public int Arity => _declaration.Parameter.Count;

        public Option<object> Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(interpreter.Globals);

            for (int i = 0; i < _declaration.Parameter.Count; i++)
            {
                environment.Define(_declaration.Parameter[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(_declaration.Body, environment);
            return Option.None<object>();
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}

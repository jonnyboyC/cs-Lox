using System.Collections.Generic;
using csLox.Parsing;
using csLox.Exceptions;
using Environment = csLox.Scoping.Environment;

namespace csLox.Interpreting
{
    internal class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function _declaration;
        private readonly Environment _closure;
        private readonly bool _isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            _closure = closure;
            _declaration = declaration;
            _isInitializer = isInitializer;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_declaration, environment, _isInitializer);
        }

        public int Arity => _declaration.Parameter.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(_closure);

            for (int i = 0; i < _declaration.Parameter.Count; i++)
            {
                environment.Define(_declaration.Parameter[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                if (_isInitializer) return _closure.GetAt(0, "this");

                return returnValue.Value;
            }

            if (_isInitializer) return _closure.GetAt(0, "this");

            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}

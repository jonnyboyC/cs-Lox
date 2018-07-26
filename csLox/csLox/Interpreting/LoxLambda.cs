using System.Collections.Generic;
using csLox.Exceptions;
using csLox.Parsing;
using Environment = csLox.Scoping.Environment;


namespace csLox.Interpreting
{
    class LoxLambda : ILoxCallable
    {
        private readonly Expr.Lambda _declaration;
        private readonly Environment _closure;

        internal LoxLambda(Expr.Lambda declaration, Environment closure)
        {
            _closure = closure;
            _declaration = declaration;
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
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<anonymous>";
        }
    }
}

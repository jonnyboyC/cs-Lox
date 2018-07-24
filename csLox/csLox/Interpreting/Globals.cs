using System;
using System.Collections.Generic;
using System.Text;
using Optional;

namespace csLox.Interpreting
{
    internal abstract class Globals : LoxCallable
    {
        public abstract int Arity { get; }

        public abstract Option<object> Call(Interpreter interpreter, List<object> arguments);

        internal class Clock : Globals
        {
            public override int Arity { get; } = 0;

            public override Option<object> Call(Interpreter interpreter, List<object> arguments)
            {
                return ((object)((double)DateTime.Now.Millisecond / 1000.0)).Some();
            }
        }
    }
}

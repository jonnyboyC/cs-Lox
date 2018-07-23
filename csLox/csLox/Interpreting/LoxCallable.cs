using Optional;
using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Interpreting
{
    interface LoxCallable
    {
        int Arity { get; }
        Option<object> Call(Interpreter interpreter, List<object> arguments);
    }
}

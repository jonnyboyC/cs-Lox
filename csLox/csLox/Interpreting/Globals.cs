﻿using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Interpreting
{
    internal abstract class Globals : LoxCallable
    {
        public abstract int Arity { get; }

        public abstract object Call(Interpreter interpreter, List<object> arguments);

        internal class Clock : Globals
        {
            public override int Arity { get; } = 0;

            public override object Call(Interpreter interpreter, List<object> arguments)
            {
                return (double)DateTime.Now.Millisecond / 1000.0;
            }
        }
    }
}
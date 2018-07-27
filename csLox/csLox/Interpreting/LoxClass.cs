using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Interpreting
{
    internal class LoxClass : ILoxCallable
    {
        public string Name { get; }

        public LoxClass(string name)
        {
            Name = name;
        }

        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

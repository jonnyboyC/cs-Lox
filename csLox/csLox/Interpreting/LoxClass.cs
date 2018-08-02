using Optional;
using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Interpreting
{
    internal class LoxClass : ILoxCallable
    {
        public string Name { get; }
        private readonly Dictionary<string, LoxFunction> _methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public Option<LoxFunction> FindMethod(LoxInstance instance, string name)
        {
            if (_methods.TryGetValue(name, out LoxFunction method))
            {
                return method.Bind(instance).Some();
            }

            return Option.None<LoxFunction>();
        }

        public int Arity
        {
            get
            {
                if (_methods.TryGetValue("init", out LoxFunction initializer))
                {
                    return initializer.Arity;
                }
                return 0;
            }
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            if (_methods.TryGetValue("init", out LoxFunction initializer))
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

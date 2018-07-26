using Optional;
using System;
using System.Collections.Generic;
using System.Text;

namespace csLox.Exceptions
{
    internal class Return : Exception
    {
        public object Value { get; }

        internal Return(object value) : base()
        {
            Value = value;
        }
    }
}

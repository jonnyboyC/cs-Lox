using System;

namespace csLox
{
    public sealed class LoxVoid
    {
        public LoxVoid()
        {
            throw new InvalidOperationException("Don't instantiate Dummy.");
        }
    }
}

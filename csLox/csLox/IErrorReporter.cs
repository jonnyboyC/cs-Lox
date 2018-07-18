using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    public interface IErrorReporter
    {
        void Error(int line, string message);
    }
}

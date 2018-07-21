using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    public interface IErrorReporter
    {
        void Report(int line, string where, string message);
    }
}

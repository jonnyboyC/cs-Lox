using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    internal interface IErrorReporter
    {
        void ReportError(int line, string where, string message);
        void ReportRuntimeError(RuntimeError error);
    }
}

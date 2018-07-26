using System;
using csLox.Exceptions;

namespace csLox
{
    internal class ConsoleReporter: IErrorReporter
    {
        public void ReportError(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
        }

        public void ReportRuntimeError(RuntimeError error)
        {
            Console.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        }
    }
}

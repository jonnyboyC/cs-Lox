using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    internal class ConsoleReporter: IErrorReporter
    {
        public void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            Lox.HadError = true;
        }
    }
}

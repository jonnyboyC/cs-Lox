using System;
using System.Collections.Generic;
using System.Text;

namespace csLox
{
    internal class ConsoleReporter: IErrorReporter
    {
        public void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
        }
    }
}

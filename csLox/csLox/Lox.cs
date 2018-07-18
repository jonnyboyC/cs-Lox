using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csLox
{
    public class Lox
    {
        public static bool HadError = false;
        private static IErrorReporter Reporter = new ConsoleReporter();

        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: csLox script.lox");
                return 64;
            }
            else if (args.Length == 1)
            {
                return RunFile(args[0]);
            }
            else
            {
                return RunPrompt();
            }
        }

        public static void Error(int line, string message)
        {
            Reporter.Error(line, message);
        }

        private static int RunFile(string path)
        {
            string script = File.ReadAllText(path);
            Run(script);

            if (HadError) return 65;
            return 0;
        }

        private static int RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                HadError = false;
            }
        }

        private static int Run(string source)
        {
            Scanner scanner = new Scanner(source);
            Token[] tokens = scanner.ScanTokens().ToArray();

            foreach (var token in tokens)
            {
                Console.WriteLine($"\t{token}");
            }
            return 0;
        }
    }
}

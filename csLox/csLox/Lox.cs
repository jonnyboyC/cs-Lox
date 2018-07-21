using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optional;
using csLox.Scanning;
using csLox.Parsing;
using csLox.Utilities;

namespace csLox
{
    public class Lox
    {
        public static bool HadError = false;
        private static IErrorReporter Reporter = new ConsoleReporter();

        public static int Main(string[] args)
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

        internal static void Error(Token token, string message)
        {
            if (token.Type == TokenType.Eof)
            {
                Reporter.Report(token.Line, " at end", message);
            }
            else
            {
                Reporter.Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        internal static void Error(int line, string message)
        {
            Reporter.Report(line, "",  message);
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
            IEnumerable<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            Option<Expr> expression = parser.Parse();

            if (HadError) return 1;

            expression.Match(
                expr => Console.WriteLine(new AstPrinter().Print(expr)),
                () => Console.WriteLine("Parser bug")
            );

            return 0;
        }
    }
}

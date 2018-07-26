using System;
using System.IO;
using System.Linq;
using csLox.Scanning;
using csLox.Parsing;
using csLox.Utilities;
using csLox.Interpreting;
using csLox.Exceptions;

namespace csLox
{
    public class Lox
    {
        private static bool HadError = false;
        private static bool HadRuntimeError = false;
        private static IErrorReporter Reporter = new ConsoleReporter();
        private static Interpreter Interpreter = new Interpreter();

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
                Reporter.ReportError(token.Line, " at end", message);
            }
            else
            {
                Reporter.ReportError(token.Line, $" at '{token.Lexeme}'", message);
            }
            HadError = true;
        }

        internal static void RuntimeError(RuntimeError error) 
        {
            Reporter.ReportRuntimeError(error);
            HadRuntimeError = true;
        }

        internal static void Error(int line, string message)
        {
            Reporter.ReportError(line, "",  message);
            HadError = true;
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
                HadRuntimeError = false;
            }
        }

        private static int Run(string source)
        {
            Scanner scanner = new Scanner(source);
            Token[] tokens = scanner.ScanTokens().ToArray();

            Parser parser = new Parser(tokens);
            Stmt[] statements = parser.Parse().ToArray();


            var astPrinter = new AstPrinter();
            foreach (Stmt stmt in statements)
            {
                foreach (string print in astPrinter.PrintStmt(stmt))
                {
                    Console.WriteLine(print);
                }
            }

            if (HadError) return 65;
            Interpreter.Interpret(statements);
            if (HadRuntimeError) return 70;

            Console.Read();
            return 0;
        }
    }
}

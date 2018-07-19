using System;
using System.IO;
using System.Collections.Generic;

namespace tools
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1) 
            {
                Console.WriteLine("Usage: generate_ast <output directory>");
                return 1;
            }
            
            string outputDir = args[0];

            DefineAst(outputDir, "Expr", "csLox.Parsing", new List<string>() 
            {
                "Binary     : Expr left, Token opCode, Expr right",
                "Grouping   : Expr expression",
                "Literal    : Object value",
                "Unary      : Token opCode, Expr right",
            });
            return 0;
        }

        private static void DefineAst(
            string outputDirectory, 
            string baseName, 
            string nameSpaceName, 
            List<string> types) 
        {
            string path = $"{outputDirectory}/{baseName}.cs";
            using (StreamWriter sw = File.CreateText(path)) 
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using csLox.Scanning;");
                sw.WriteLine("");
                sw.WriteLine($"namespace {nameSpaceName}");
                sw.WriteLine("{");

                sw.WriteLine($"    internal abstract class {baseName} {{ }}");

                foreach (string type in types) 
                {
                    string[] typeSplit = type.Split(':');
                    string className = typeSplit[0].Trim();
                    string fields = typeSplit[1].Trim();
                    sw.WriteLine("");

                    DefineType(sw, baseName, className, fields);
                }


                sw.WriteLine("}");
            }
        }

        private static void DefineType(
            StreamWriter sw,
            string baseName,
            string className,
            string fieldList) 
        {
            sw.WriteLine($"    internal class {className} : {baseName}");
            sw.WriteLine("    {");

            string[] fields = fieldList.Split(",");
            List<(string, string)> names = new List<(string, string)>();
            foreach (string field in fields) 
            {
                string[] typeSplit = field.Trim().Split(" ");
                Console.WriteLine(field);
                string type =typeSplit[0].Trim();
                string name = typeSplit[1].Trim();
                string propertyName = char.ToUpper(name[0]) + name.Substring(1);
                names.Add((propertyName, name));

                sw.WriteLine($"        public {type} {propertyName} {{ get; }}");
            }

            sw.WriteLine($"        {className}({fieldList})");
            sw.WriteLine("        {");

            foreach (var (propertyName, name) in names) 
            {
                sw.WriteLine($"            {propertyName} = {name};");
            }
            sw.WriteLine("        }");
            sw.WriteLine("    }");
        }
    }
}

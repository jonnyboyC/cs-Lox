using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace tools
{
    class GenerateAst
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
                    "Assign     : Token name, Expr value",
                    "Grouping   : Expr expression",
                    "Lambda     : List<Token> parameter, List<Stmt> body",
                    "Logical    : Expr left, Token opCode, Expr right",
                    "Literal    : Object value",
                    "Conditional: Expr condition, Expr trueExpr, Expr falseExpr",
                    "Binary     : Expr left, Token opCode, Expr right",
                    "Call       : Expr callee, Token paren, List<Expr> arguments",
                    "Unary      : Token opCode, Expr right",
                    "Variable   : Token name"
                },
                "csLox.Scanning"
            );

            DefineAst(outputDir, "Stmt", "csLox.Parsing", new List<string>()
                {
                    "Block          : List<Stmt> statements",
                    "Class          : Token name, LIst<Stmt.Function> methods",
                    "ExpressionStmt : Expr expression",
                    "Function       : Token name, List<Token> parameter, List<Stmt> body",
                    "If             : Expr condition, Stmt thenBranch, Option<Stmt> elseBranch",
                    "Print          : Expr expression",
                    "Return         : Token keyword, Option<Expr> value",
                    "Var            : Token name, Option<Expr> initializer",
                    "While          : Expr condition, Stmt body",
                    "Break          : ",
                },
                "csLox.Scanning",
                "Optional"
            );
            return 0;
        }

        private static void DefineVisitor(
            StreamWriter sw,
            string baseName,
            List<AstType> types)
        {
            sw.WriteLine("        internal interface Visitor<T> ");
            sw.WriteLine("        {");

            foreach (AstType type in types)
            {
                sw.WriteLine($"            T Visit{type.Name}{baseName}({type.Name} {baseName.ToLower()});");
            }

            sw.WriteLine("        }");
        }

        private static void DefineAst(
            string outputDirectory, 
            string baseName, 
            string nameSpaceName, 
            List<string> typeDefinitions,
            params string[] additionalDependencies) 
        {
            string path = $"{outputDirectory}/{baseName}.cs";
            List<AstType> types = typeDefinitions.Select(t => new AstType(t)).ToList();

            using (StreamWriter sw = File.CreateText(path)) 
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");

                foreach (string dependency in additionalDependencies)
                {
                    sw.WriteLine($"using {dependency};");
                }

                sw.WriteLine("");
                sw.WriteLine($"namespace {nameSpaceName}");
                sw.WriteLine("{");

                sw.WriteLine($"    internal abstract class {baseName}");
                sw.WriteLine("    {");
                sw.WriteLine("        internal abstract T Accept<T>(Visitor<T> visitor);");
                sw.WriteLine();
                DefineVisitor(sw, baseName, types);
                sw.WriteLine();

                foreach (AstType type in types)
                {
                    sw.WriteLine("");
                    DefineType(sw, type, baseName);
                }

                sw.WriteLine("    }");
                sw.WriteLine("}");
            }
        }

        private static void DefineType(
            StreamWriter sw,
            AstType astType,
            string baseName) 
        {
            sw.WriteLine($"        internal class {astType.Name} : {baseName}");
            sw.WriteLine("        {");

            foreach (AstField astField in astType.Fields) 
            {
                sw.WriteLine($"            internal {astField.Type} {astField.PropertyName} {{ get; }}");
            }

            sw.WriteLine($"            internal {astType.Name}({astType.FieldList})");
            sw.WriteLine("            {");

            foreach (AstField field in astType.Fields) 
            {
                sw.WriteLine($"                {field.PropertyName} = {field.Name};");
            }
            sw.WriteLine("            }");

            sw.WriteLine("            internal override T Accept<T>(Visitor<T> visitor)");
            sw.WriteLine("            {");
            sw.WriteLine($"                return visitor.Visit{astType.Name}{baseName}(this);");
            sw.WriteLine("            }");
            sw.WriteLine("        }");
        }

        internal class AstType
        {
            public string Name { get; }
            public List<AstField> Fields { get; }
            public string FieldList => string.Join(", ", Fields.Select(f => f.ToString())); 

            public AstType(string type)
            {
                string[] typeSplit = type.Split(':');
                string fieldList = typeSplit[1].Trim();

                string[] fields = fieldList.Split(",");

                Name = typeSplit[0].Trim();
                Fields = fields.Where(f => !string.IsNullOrEmpty(f)).Select(f => new AstField(f)).ToList();
            }
        }

        internal class AstField
        {
            public string Type { get; }
            public string Name { get; }
            public string PropertyName { get; }

            public AstField(string field)
            {
                string[] typeSplit = field.Trim().Split(" ");

                Type = typeSplit[0].Trim();
                Name = typeSplit[1].Trim();
                PropertyName = char.ToUpper(Name[0]) + Name.Substring(1);
            }

            public override string ToString()
            {
                return $"{Type} {Name}";
            }
        }
    }
}

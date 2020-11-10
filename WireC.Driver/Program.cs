using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using CommandLine;

using WireC.AST;
using WireC.BackEnd;
using WireC.Common;
using WireC.FrontEnd;
using WireC.MiddleEnd;

using Parser = CommandLine.Parser;

namespace WireC.Driver
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Parser.Default.ParseArguments<Options>(args).WithParsed(
                options =>
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                        !options.OutputFile.EndsWith(".exe"))
                        options.OutputFile += ".exe";

                    var context = new Context(options);
                    CompileFile(context);
                }
            );
        }

        private static void CompileFile(Context context)
        {
            var sourceCode = File.ReadAllText(context.Options.InputFile, Encoding.UTF8);
            context.SourceCode = sourceCode;
            CompileString(context, sourceCode);
        }

        private static void CompileString(Context context, string sourceCode)
        {
            var tokens = new Lexer(sourceCode).Tokenize();
            var abstractSyntaxTree = FrontEnd.Parser.Parse(context, tokens);
            if (context.ErrorCount > 0) TerminateCompilation(context, 1);

            var abstractSyntaxTreeContext = new ASTContext();

            new SemanticAnalyzer(context, abstractSyntaxTree, abstractSyntaxTreeContext).Analyze();
            if (context.ErrorCount > 0) TerminateCompilation(context, 1);

            var destinationCode = new CodeGenerator(abstractSyntaxTree, abstractSyntaxTreeContext)
                .GenerateCode();

            var cOutputFile = context.Options.OutputFile.EndsWith(".exe") &&
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? context.Options.OutputFile[..^4] + ".cpp"
                    : context.Options.OutputFile + ".cpp";
            File.WriteAllText(cOutputFile, destinationCode);

            Process.Start(
                context.Options.CCompiler,
                $"{cOutputFile} -o {context.Options.OutputFile}"
            );

            TerminateCompilation(context, 0);
        }

        private static void TerminateCompilation(Context context, int exitCode)
        {
            Console.Error.WriteLine(
                $"compilation finished with {context.ErrorCount} errors " +
                $"and {context.WarningCount} warnings " +
                $"in {(float) context.CompilationTime.Milliseconds / 1000}s"
            );
            Environment.Exit(exitCode);
        }
    }
}
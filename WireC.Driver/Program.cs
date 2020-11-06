using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using CommandLine;

using WireC.Common;
using WireC.FrontEnd;

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
            var ast = FrontEnd.Parser.Parse(context, tokens);
            if (context.ErrorCount > 0) TerminateCompilation(context, 1);

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

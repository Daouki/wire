using System.Runtime.InteropServices;

using CommandLine;

namespace WireC.Common
{
    public class Options
    {
        [Value(0, Required = true, MetaName = "INPUT", HelpText = "The file to be compiled")]
        public string InputFile { get; set; }

        [Option('o', HelpText = "Name of the output file")]
        public string OutputFile { get; set; } =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "out.exe" : "out";

        [Option('c', "c-compiler", HelpText = "The C++ compiler to use")]
        public string CCompiler { get; set; } = "g++";
    }
}
using CommandLine;

namespace WireC.Common
{
    public class Options
    {
        [Value(0, Required = true, MetaName = "INPUT", HelpText = "The file to be compiled")]
        public string InputFile { get; set; }

        [Option(
            'o',
            Default = "out",
            HelpText = "Name of the output file")]
        public string OutputFile { get; set; }

        [Option('c', "c-compiler", Default = "g++", HelpText = "The C++ compiler to use")]
        public string CCompiler { get; set; }

        [Option(
            "first-error-only",
            Default = false,
            HelpText = "Print only the first encountered error message")]
        public bool FirstErrorOnly { get; set; }

        [Option(
            "no-warnings",
            Default = false,
            HelpText = "Do not emit any warning messages")]
        public bool NoWarnings { get; set; }
    }
}

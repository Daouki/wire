using System;

namespace WireC.Common
{
    public class Context
    {
        private const string _pipe = " | ";

        public Context(Options options)
        {
            Options = options;
            StartTime = DateTime.Now;
        }

        public Options Options { get; set; }

        /// <summary>
        /// The currently compiled source file.
        /// </summary>
        public SourceFile SourceFile { get; set; }

        /// <summary>
        /// Number of reported errors.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Number of reported warnings.
        /// </summary>
        public int WarningCount { get; private set; }

        public DateTime StartTime { get; }

        /// <summary>
        /// Gets the time that elapsed since the compilation started.
        /// </summary>
        public TimeSpan CompilationTime => DateTime.Now.Subtract(StartTime);

        public void Error(SourceSpan span, string message)
        {
            ErrorCount++;
            Report(span, "error", ConsoleColor.Red, message);
        }

        public void Warning(SourceSpan span, string message)
        {
            WarningCount++;
            Report(span, "warning", ConsoleColor.Yellow, message);
        }

        private void Report(
            SourceSpan span,
            string header,
            ConsoleColor accentColor,
            string message)
        {
            Console.Error.WriteLine($"{Options.InputFile}:{span.Line}:{span.Column}");

            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = accentColor;
            Console.Error.Write($"{header}: ");
            Console.ForegroundColor = originalForegroundColor;
            Console.Error.WriteLine($"{message}");

            var linesInSpan = SourceFile.GetLineCountInSpan(span);
            if (linesInSpan == 1) WriteLineWithUnderline(span, accentColor);
            else WriteLinesWithUnderline(span);

            Console.Write(Environment.NewLine);
        }

        private void WriteLineWithUnderline(SourceSpan underlineSpan, ConsoleColor underlineColor)
        {
            if (underlineSpan == null) throw new ArgumentException(nameof(underlineSpan));

            var (lineStart, lineEnd) = SourceFile.GetLineBounds(underlineSpan.Start);
            var line = SourceFile.ReadSpan(lineStart, lineEnd);
            var underlineOffset = GetIntLength(underlineSpan.Line) + _pipe.Length +
                (underlineSpan.Start - lineStart);

            Console.Error.WriteLine($"{underlineSpan.Line}{_pipe}{line}");

            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = underlineColor;
            for (var i = 0; i < underlineOffset; i++) Console.Write(' ');
            for (var i = 0; i < underlineSpan.Length; i++) Console.Write('^');
            Console.ForegroundColor = originalForegroundColor;

            Console.Write(Environment.NewLine);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void WriteLinesWithUnderline(SourceSpan span)
        {
            if (span == null) throw new ArgumentException(nameof(span));
            Console.WriteLine("writing of multi-line spans is not supported yet");
            Console.Write(Environment.NewLine);
        }

        private static int GetIntLength(int i) => i.ToString().Length;
    }
}

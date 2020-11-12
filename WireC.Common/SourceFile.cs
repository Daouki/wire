namespace WireC.Common
{
    public class SourceFile
    {
        public SourceFile(string filePath, string sourceCode)
        {
            FilePath = filePath;
            SourceCode = sourceCode;
        }

        /// <summary>
        /// Path to the source file, as given to the compiler.
        /// </summary>
        /// <note>
        /// This path may be relative or absolute, we don't know.
        /// </note>
        public string FilePath { get; }

        /// <summary>
        /// The complete source code that came from the source file.
        /// </summary>
        public string SourceCode { get; }

        /// <summary>
        /// Reads the given source span into a string.
        /// </summary>
        public string ReadSpan(SourceSpan span) => ReadSpan(span.Start, span.End);

        /// <summary>
        /// Reads the given source span into a string.
        /// </summary>
        public string ReadSpan(int start, int end) => SourceCode[start..end];

        /// <summary>
        /// Gets the number of newline \n characters in the given source code span.
        /// </summary>
        public int GetLineCountInSpan(SourceSpan span)
        {
            var lines = 1;
            for (var i = span.Start; i < span.End && i < SourceCode.Length; i++)
            {
                if (SourceCode[i] == '\n')
                    lines++;
            }

            return lines;
        }

        /// <summary>
        /// Gets the start and end offsets (relative to the beginning of the source code) of the
        /// given line number.
        /// </summary>
        public (int, int) GetLineBounds(int position)
        {
            var start = position;
            while (start > 0 && SourceCode[start] != '\n') start--;
            if (start != 0) start++;

            var end = position;
            while (end < SourceCode.Length && SourceCode[end] != '\n') end++;

            return (start, end);
        }
    }
}

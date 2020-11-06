using System.Collections.Generic;

using WireC.Common;

namespace WireC.AST
{
    public class Block
    {
        public Block(Token openingBrace, Token closingBrace, List<IStatement> statements)
        {
            Span = SourceSpan.Merge(openingBrace.Span, closingBrace.Span);
            OpeningBrace = openingBrace;
            ClosingBrace = closingBrace;
            Statements = statements;
        }

        public Token OpeningBrace { get; }
        public Token ClosingBrace { get; }
        public List<IStatement> Statements { get; }
        public SourceSpan Span { get; }
    }
}
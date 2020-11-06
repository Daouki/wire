namespace WireC.Common
{
    public class Token
    {
        public TokenKind Kind { get; set; }
        public SourceSpan Span { get; set; }
        public string Lexeme { get; set; }

        public override string ToString() => Lexeme;
    }
}
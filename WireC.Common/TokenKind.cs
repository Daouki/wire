namespace WireC.Common
{
    public enum TokenKind
    {
        Identifier,
        Integer,

        Fn,
        Return,

        Asterisk,
        Colon,
        LeftBrace,
        LeftParenthesis,
        Minus,
        Plus,
        RightBrace,
        RightParenthesis,
        Semicolon,
        Slash,

        Invalid,
        EndOfFile,
    }
}

namespace WireC.Common
{
    public enum TokenKind
    {
        Identifier,
        Integer,

        Fn,
        Return,

        Colon,
        LeftBrace,
        LeftParenthesis,
        Minus,
        Plus,
        RightBrace,
        RightParenthesis,
        Semicolon,

        Invalid,
        EndOfFile,
    }
}

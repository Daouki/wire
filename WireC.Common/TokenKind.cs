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
        RightBrace,
        RightParenthesis,
        Semicolon,

        Invalid,
        EndOfFile,
    }
}
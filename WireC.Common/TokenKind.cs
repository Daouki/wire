namespace WireC.Common
{
    public enum TokenKind
    {
        Identifier,
        Integer,

        Fn,
        Return,
        Var,

        Asterisk,
        Colon,
        Equal,
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

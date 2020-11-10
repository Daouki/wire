namespace WireC.Common
{
    public enum TokenKind
    {
        Identifier,
        Integer,

        False,
        Fn,
        Return,
        True,
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
namespace WireC.Common
{
    public enum TokenKind
    {
        Identifier,
        Integer,

        Assert,
        False,
        Fn,
        Return,
        True,
        Var,

        Asterisk,
        Bang,
        Colon,
        Equal,
        EqualEqual,
        Greater,
        GreaterEqual,
        LeftBrace,
        LeftParenthesis,
        Less,
        LessEqual,
        LessGreater,
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
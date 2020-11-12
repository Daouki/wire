using System.ComponentModel;

namespace WireC.Common
{
    public enum TokenKind
    {
        [Description("identifier")]
        Identifier,

        [Description("integer")]
        Integer,

        [Description("\"assert\"")]
        Assert,

        [Description("\"false\"")]
        False,

        [Description("\"fn\"")]
        Fn,

        [Description("\"return\"")]
        Return,

        [Description("\"true\"")]
        True,

        [Description("\"var\"")]
        Var,

        [Description("\"*\"")]
        Asterisk,

        [Description("\"!\"")]
        Bang,

        [Description("\":\"")]
        Colon,

        [Description("\"=\"")]
        Equal,

        [Description("\"==\"")]
        EqualEqual,

        [Description("\">\"")]
        Greater,

        [Description("\">=\"")]
        GreaterEqual,

        [Description("\"}\"")]
        LeftBrace,

        [Description("\"(\"")]
        LeftParenthesis,

        [Description("\"<\"")]
        Less,

        [Description("\"<=\"")]
        LessEqual,

        [Description("\"<>\"")]
        LessGreater,

        [Description("\"-\"")]
        Minus,

        [Description("\"+\"")]
        Plus,

        [Description("\"}\"")]
        RightBrace,

        [Description("\")\"")]
        RightParenthesis,

        [Description("\";\"")]
        Semicolon,

        [Description("\"/\"")]
        Slash,

        [Description("\"INVALID\"")]
        Invalid,

        [Description("\"END_OF_FILE\"")]
        EndOfFile,
    }
}

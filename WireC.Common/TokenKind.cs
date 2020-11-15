using System.ComponentModel;

namespace WireC.Common
{
    public enum TokenKind
    {
        // Literals.

        [Description("identifier")]
        Identifier,

        [Description("integer")]
        Integer,

        // Keywords.

        [Description("\"assert\"")]
        Assert,

        [Description("\"else\"")]
        Else,

        [Description("\"false\"")]
        False,

        [Description("\"fn\"")]
        Fn,

        [Description("\"if\"")]
        If,

        [Description("\"return\"")]
        Return,

        [Description("\"true\"")]
        True,

        [Description("\"var\"")]
        Var,

        [Description("\"while\"")]
        While,

        // Operators.

        [Description("\"&\"")]
        Ampersand,

        [Description("\"&&\"")]
        AmpersandAmpersand,

        [Description("\"*\"")]
        Asterisk,

        [Description("\"!\"")]
        Bang,

        [Description("\"^\"")]
        Caret,

        [Description("\":\"")]
        Colon,

        [Description(@""":=""")]
        ColonEqual,

        [Description("\",\"")]
        Comma,

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

        [Description("\"|\"")]
        Pipe,

        [Description("\"||\"")]
        PipePipe,

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

        [Description("\"~\"")]
        Tilde,

        // Special.

        [Description("\"INVALID\"")]
        Invalid,

        [Description("\"END_OF_FILE\"")]
        EndOfFile,
    }
}

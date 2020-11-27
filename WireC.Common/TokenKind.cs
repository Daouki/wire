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

        [Description("float")]
        Float,

        // Keywords.

        [Description("\"assert\"")]
        Assert,

        [Description(@"""break""")]
        Break,

        [Description(@"""continue""")]
        Continue,

        [Description(@"""elif""")]
        Elif,

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

        [Description(@""">>""")]
        GreaterGreater,

        [Description(@"""[""")]
        LeftBracket,

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

        [Description(@"""<<""")]
        LessLess,

        [Description("\"-\"")]
        Minus,

        [Description("\"%\"")]
        Percent,

        [Description("\"|\"")]
        Pipe,

        [Description("\"||\"")]
        PipePipe,

        [Description("\"+\"")]
        Plus,

        [Description(@"""]""")]
        RightBracket,

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
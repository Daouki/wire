using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.FrontEnd
{
    public class ExpressionParser
    {
        public static IExpression ParseExpression(ParserState state) =>
            ParsePrimaryExpression(state);

        private static IExpression ParsePrimaryExpression(ParserState state)
        {
            if (state.Consume(TokenKind.Integer)) return new IntegerLiteral(state.Previous());
            throw new ParseException(
                state.Current().Span,
                $"expected primary expression, but found \"{state.Current().Lexeme}\""
            );
        }
    }
}
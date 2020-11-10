using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.FrontEnd
{
    public static class ExpressionParser
    {
        private static readonly TokenKind[] _prefixOperators =
        {
            TokenKind.Minus,
            TokenKind.Plus,
        };

        private static readonly TokenKind[] _infixOperators =
        {
            TokenKind.Asterisk,
            TokenKind.Minus,
            TokenKind.Plus,
            TokenKind.Slash,
        };

        public static IExpression ParseExpression(ParserState state) =>
            ParseInfixOperation(state);

        private static IExpression ParseInfixOperation(ParserState state)
        {
            var expression = ParsePostfixOperation(state);
            while (!state.IsAtEnd() && state.Consume(_infixOperators))
            {
                var @operator = InfixOperator.FromToken(state.Previous());
                var rightOperand = ParsePostfixOperation(state);
                expression = new InfixOperation(
                    state.NodeIdGenerator.GetNextId(),
                    @operator,
                    expression,
                    rightOperand
                );
            }

            return expression;
        }

        private static IExpression ParsePostfixOperation(ParserState state)
        {
            var operand = ParsePrefixOperation(state);
            if (state.Consume(TokenKind.LeftParenthesis)) return ParseFunctionCall(state, operand);
            return operand;
        }

        private static IExpression ParsePrefixOperation(ParserState state)
        {
            if (!state.Consume(_prefixOperators)) return ParsePrimaryExpression(state);
            var @operator = PrefixOperator.FromToken(state.Previous());
            var operand = ParsePrefixOperation(state);
            return new PrefixOperation(state.NodeIdGenerator.GetNextId(), @operator, operand);
        }

        private static IExpression ParseFunctionCall(ParserState state, IExpression callee)
        {
            state.ConsumeOrError(TokenKind.RightParenthesis);
            var span = SourceSpan.Merge(callee.Span, state.Previous().Span);
            return new FunctionCall(state.NodeIdGenerator.GetNextId(), span, callee);
        }

        private static IExpression ParsePrimaryExpression(ParserState state)
        {
            if (state.Consume(TokenKind.False))
            {
                return new BooleanLiteral(
                    state.NodeIdGenerator.GetNextId(),
                    state.Previous().Span,
                    false
                );
            }

            if (state.Consume(TokenKind.Identifier))
                return new IdentifierLiteral(state.NodeIdGenerator.GetNextId(), state.Previous());

            if (state.Consume(TokenKind.Integer))
                return new IntegerLiteral(state.NodeIdGenerator.GetNextId(), state.Previous());

            if (state.Consume(TokenKind.True))
            {
                return new BooleanLiteral(
                    state.NodeIdGenerator.GetNextId(),
                    state.Previous().Span,
                    true
                );
            }

            throw new ParseException(
                state.Current().Span,
                $"expected primary expression, but found \"{state.Current().Lexeme}\""
            );
        }
    }
}

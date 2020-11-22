using System.Collections.Generic;

using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.FrontEnd
{
    public static class ExpressionParser
    {
        private static readonly TokenKind[] _prefixOperators =
        {
            TokenKind.Bang,
            TokenKind.Minus,
            TokenKind.Plus,
            TokenKind.Tilde,
        };

        private static readonly TokenKind[] _infixOperators =
        {
            TokenKind.Ampersand,
            TokenKind.AmpersandAmpersand,
            TokenKind.Asterisk,
            TokenKind.Caret,
            TokenKind.EqualEqual,
            TokenKind.Greater,
            TokenKind.GreaterEqual,
            TokenKind.GreaterGreater,
            TokenKind.Less,
            TokenKind.LessEqual,
            TokenKind.LessGreater,
            TokenKind.LessLess,
            TokenKind.Minus,
            TokenKind.Percent,
            TokenKind.Pipe,
            TokenKind.PipePipe,
            TokenKind.Plus,
            TokenKind.Slash,
            TokenKind.Tilde,
        };

        public static IExpression ParseExpression(ParserState state) =>
            ParseInfixOperation(state);

        private static IExpression ParseInfixOperation(ParserState state)
        {
            var expression = ParsePostfixOperation(state);
            while (!state.IsAtEnd() && state.Consume(_infixOperators))
            {
                var @operator = state.Previous();
                var rightOperand = ParsePostfixOperation(state);
                expression = new InfixOperation(
                    state.NodeIdGenerator.GetNextId(),
                    @operator,
                    expression,
                    rightOperand);
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
            var @operator = state.Previous();
            var operand = ParsePrefixOperation(state);
            return new PrefixOperation(state.NodeIdGenerator.GetNextId(), @operator, operand);
        }

        private static IExpression ParseFunctionCall(ParserState state, IExpression callee)
        {
            var arguments = new List<IExpression>();
            if (state.Consume(TokenKind.RightParenthesis))
            {
                return new FunctionCall(
                    state.NodeIdGenerator.GetNextId(),
                    SourceSpan.Merge(callee.Span, state.Previous().Span),
                    callee,
                    arguments);
            }

            do
            {
                arguments.Add(ParseExpression(state));
            } while (!state.IsAtEnd() && state.Consume(TokenKind.Comma));

            state.ConsumeOrError(TokenKind.RightParenthesis);
            var span = SourceSpan.Merge(callee.Span, state.Previous().Span);
            return new FunctionCall(state.NodeIdGenerator.GetNextId(), span, callee, arguments);
        }

        private static IExpression ParsePrimaryExpression(ParserState state)
        {
            var nodeId = state.NodeIdGenerator.GetNextId();

            if (state.Consume(TokenKind.False))
                return new BooleanLiteral(nodeId, state.Previous().Span, false);

            if (state.Consume(TokenKind.Identifier))
                return new IdentifierLiteral(nodeId, state.Previous());

            if (state.Consume(TokenKind.Float))
                return new FloatLiteral(nodeId, state.Previous());

            if (state.Consume(TokenKind.Integer))
                return new IntegerLiteral(nodeId, state.Previous());

            if (state.Consume(TokenKind.True))
                return new BooleanLiteral(nodeId, state.Previous().Span, true);

            if (state.Consume(TokenKind.LeftParenthesis))
                return ParseParenthesizedExpression(state);

            if (state.Consume(TokenKind.LeftBracket))
                return ParseArrayLiteral(state);

            throw new ParseException(
                state.Current().Span,
                $"expected primary expression, but found \"{state.Current().Lexeme}\"");
        }

        private static IExpression ParseArrayLiteral(ParserState state)
        {
            var nodeId = state.NodeIdGenerator.GetNextId();
            SourceSpan span;

            var spanStart = state.Previous().Span;
            var elements = new List<IExpression>();

            if (state.Consume(TokenKind.RightBracket))
            {
                throw new ParseException(
                    state.Previous().Span,
                    "empty array literals are not allowed");
            }

            do
            {
                elements.Add(ParseExpression(state));
            } while (state.Consume(TokenKind.Comma));

            state.ConsumeOrError(TokenKind.RightBracket);

            var spanEnd = state.Previous().Span;
            span = SourceSpan.Merge(spanStart, spanEnd);
            return new ArrayLiteral(nodeId, span, elements);
        }

        private static IExpression ParseParenthesizedExpression(ParserState state)
        {
            var startSpan = state.Previous().Span;
            var expression = ParseExpression(state);
            state.ConsumeOrError(TokenKind.RightParenthesis);
            var endSpan = state.Previous().Span;
            return new ParenthesizedExpression(
                state.NodeIdGenerator.GetNextId(),
                SourceSpan.Merge(startSpan, endSpan),
                expression);
        }
    }
}
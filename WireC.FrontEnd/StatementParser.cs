using System;
using System.Collections.Generic;
using System.Linq;

using WireC.AST;
using WireC.AST.Statements;
using WireC.Common;

namespace WireC.FrontEnd
{
    public static class StatementParser
    {
        private static readonly Dictionary<TokenKind, StatementParseRule> _parseRules =
            new Dictionary<TokenKind, StatementParseRule>
            {
                {
                    TokenKind.Fn,
                    new StatementParseRule
                    {
                        ParserFunction = ParseFunctionDefinition,
                        EndsWithSemicolon = false,
                    }
                },
                {
                    TokenKind.Return,
                    new StatementParseRule
                    {
                        ParserFunction = ParseReturnStatement,
                        EndsWithSemicolon = true,
                    }
                },
            };

        private static readonly IEnumerable<TokenKind> _synchronizationPoints = _parseRules.Keys;

        private static IStatement ParseStatement(Context context, ParserState state)
        {
            if (!_parseRules.TryGetValue(state.Current().Kind, out var parseRule))
            {
                var current = state.Current();
                throw new ParseException(
                    current.Span,
                    $"expected a beginning of a statement, but found {current}"
                );
            }

            state.Advance();
            var statement = parseRule.ParserFunction.Invoke(context, state);
            if (parseRule.EndsWithSemicolon) state.ConsumeOrError(TokenKind.Semicolon);
            return statement;
        }

        private static IStatement ParseFunctionDefinition(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            var name = state.ConsumeOrError(TokenKind.Identifier);
            state.ConsumeOrError(TokenKind.LeftParenthesis);
            state.ConsumeOrError(TokenKind.RightParenthesis);
            state.ConsumeOrError(TokenKind.Colon);
            var returnTypeSignature = TypeSignatureParser.ParseTypeSignature(state);
            var body = ParseBlock(context, state);
            var endSpan = state.Previous().Span;
            var span = SourceSpan.Merge(startSpan, endSpan);
            return new FunctionDefinition(span, name, body, returnTypeSignature);
        }

        private static IStatement ParseReturnStatement(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            if (state.Current().Kind == TokenKind.Semicolon)
                return new ReturnStatement(startSpan, null);
            var expression = ExpressionParser.ParseExpression(state);
            var endSpan = state.Previous().Span;
            var span = SourceSpan.Merge(startSpan, endSpan);
            return new ReturnStatement(span, expression);
        }

        private static Block ParseBlock(Context context, ParserState state)
        {
            var openingBrace = state.ConsumeOrError(TokenKind.LeftBrace);
            var statements = ParseStatementsUntil(context, state, TokenKind.RightBrace);
            var closingBrace = state.Previous();
            return new Block(openingBrace, closingBrace, statements);
        }

        public static List<IStatement> ParseStatementsUntil(Context context, ParserState state,
            TokenKind kind)
        {
            var statements = new List<IStatement>();
            while (!state.IsAtEnd() && !state.Consume(kind))
            {
                try
                {
                    statements.Add(ParseStatement(context, state));
                }
                catch (ParseException e)
                {
                    context.Error(e.Span, e.Message);
                    Synchronize(state);
                }
            }

            return statements;
        }

        private static void Synchronize(ParserState state)
        {
            state.Advance();
            while (!state.IsAtEnd())
            {
                if (state.Previous().Kind == TokenKind.Semicolon) return;
                if (_synchronizationPoints.Any(kind => state.Current().Kind == kind)) return;
                state.Advance();
            }
        }

        private class StatementParseRule
        {
            public Func<Context, ParserState, IStatement> ParserFunction { get; set; }
            public bool EndsWithSemicolon { get; set; }
        }
    }
}

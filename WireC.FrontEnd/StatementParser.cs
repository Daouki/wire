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
                    TokenKind.Assert,
                    new StatementParseRule
                    {
                        ParserFunction = ParseAssertStatement,
                        EndsWithSemicolon = true,
                    }
                },
                {
                    TokenKind.Fn,
                    new StatementParseRule
                    {
                        ParserFunction = ParseFunctionDefinition,
                        EndsWithSemicolon = false,
                    }
                },
                {
                    TokenKind.If,
                    new StatementParseRule
                    {
                        ParserFunction = ParseIfStatement,
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
                {
                    TokenKind.Var,
                    new StatementParseRule
                    {
                        ParserFunction = ParseVariableDefinition,
                        EndsWithSemicolon = true,
                    }
                },
            };

        private static readonly IEnumerable<TokenKind> _synchronizationPoints = _parseRules.Keys;

        private static IStatement ParseIfStatement(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            var condition = ExpressionParser.ParseExpression(state);
            var thenBlock = ParseBlock(context, state);
            var elseBlock = state.Consume(TokenKind.Else) ? ParseBlock(context, state) : null;
            return new IfStatement(
                state.NodeIdGenerator.GetNextId(),
                SourceSpan.Merge(startSpan, elseBlock != null ? elseBlock.Span : thenBlock.Span),
                condition,
                thenBlock,
                elseBlock
            );
        }

        private static IStatement ParseAssertStatement(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            state.ConsumeOrError(TokenKind.LeftParenthesis);
            var condition = ExpressionParser.ParseExpression(state);
            state.ConsumeOrError(TokenKind.RightParenthesis);
            var endSpan = state.Previous().Span;
            return new AssertStatement(
                state.NodeIdGenerator.GetNextId(),
                SourceSpan.Merge(startSpan, endSpan),
                condition
            );
        }

        private static IStatement ParseVariableDefinition(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            var identifier = state.ConsumeOrError(TokenKind.Identifier);
            var typeSignature = state.Consume(TokenKind.Colon)
                ? TypeSignatureParser.ParseTypeSignature(state)
                : null;

            SourceSpan endSpan;
            SourceSpan span;

            if (state.Consume(TokenKind.Equal))
            {
                var initializer = ExpressionParser.ParseExpression(state);
                endSpan = state.Previous().Span;
                span = SourceSpan.Merge(startSpan, endSpan);
                return new VariableDefinition(
                    state.NodeIdGenerator.GetNextId(),
                    span,
                    identifier,
                    typeSignature,
                    initializer
                );
            }

            if (typeSignature == null)
                throw new ParseException(identifier.Span, "type signature needed");

            endSpan = state.Previous().Span;
            span = SourceSpan.Merge(startSpan, endSpan);
            return new VariableDefinition(
                state.NodeIdGenerator.GetNextId(),
                span,
                identifier,
                typeSignature,
                null
            );
        }

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
            ConsumeUnnecessarySemicolons(context, state);
            return statement;
        }

        private static IStatement ParseFunctionDefinition(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            var name = state.ConsumeOrError(TokenKind.Identifier);
            state.ConsumeOrError(TokenKind.LeftParenthesis);
            state.ConsumeOrError(TokenKind.RightParenthesis);
            var returnTypeSignature = state.Consume(TokenKind.Colon)
                ? TypeSignatureParser.ParseTypeSignature(state)
                : null;
            var body = ParseBlock(context, state);
            var endSpan = state.Previous().Span;
            var span = SourceSpan.Merge(startSpan, endSpan);
            return new FunctionDefinition(
                state.NodeIdGenerator.GetNextId(),
                span,
                name,
                body,
                returnTypeSignature
            );
        }

        private static IStatement ParseReturnStatement(Context context, ParserState state)
        {
            var startSpan = state.Previous().Span;
            if (state.Current().Kind == TokenKind.Semicolon)
                return new ReturnStatement(state.NodeIdGenerator.GetNextId(), startSpan, null);
            var expression = ExpressionParser.ParseExpression(state);
            var endSpan = state.Previous().Span;
            var span = SourceSpan.Merge(startSpan, endSpan);
            return new ReturnStatement(state.NodeIdGenerator.GetNextId(), span, expression);
        }

        private static Block ParseBlock(Context context, ParserState state)
        {
            var openingBrace = state.ConsumeOrError(TokenKind.LeftBrace);
            var statements = ParseStatementsUntil(context, state, TokenKind.RightBrace);
            var closingBrace = state.Previous();
            return new Block(openingBrace, closingBrace, statements);
        }

        public static List<IStatement> ParseStatementsUntil(
            Context context,
            ParserState state,
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

        private static void ConsumeUnnecessarySemicolons(Context context, ParserState state)
        {
            while (!state.IsAtEnd() && state.Consume(TokenKind.Semicolon))
            {
                var firstSemicolon = state.Previous();
                var lastSemicolon = firstSemicolon;
                while (state.Consume(TokenKind.Semicolon)) lastSemicolon = state.Previous();
                var semicolonsSpan = SourceSpan.Merge(firstSemicolon.Span, lastSemicolon.Span);
                var message = firstSemicolon.Span.Start == lastSemicolon.Span.Start
                    ? "unnecessary trailing semicolon"
                    : "unnecessary trailing semicolons";
                context.Warning(semicolonsSpan, message);
            }
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
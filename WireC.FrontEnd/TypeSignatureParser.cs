using WireC.AST;
using WireC.AST.TypeSignatures;
using WireC.Common;

namespace WireC.FrontEnd
{
    public static class TypeSignatureParser
    {
        public static ITypeSignature ParseTypeSignature(ParserState state)
        {
            if (state.Consume(TokenKind.Caret)) return ParsePointerSignature(state);
            if (state.Consume(TokenKind.LeftBracket)) return ParseArraySignature(state);
            return ParseTypeName(state);
        }

        private static ITypeSignature ParsePointerSignature(ParserState state)
        {
            var spanStart = state.Previous().Span;
            var underlyingType = ParseTypeSignature(state);
            var span = SourceSpan.Merge(spanStart, underlyingType.Span);
            return new PointerSignature(span, underlyingType);
        }

        private static ITypeSignature ParseArraySignature(ParserState state)
        {
            var spanStart = state.Previous().Span;
            var underlyingType = ParseTypeSignature(state);
            state.ConsumeOrError(TokenKind.Semicolon);
            var length = int.Parse(state.ConsumeOrError(TokenKind.Integer).Lexeme);
            if (length <= 0)
            {
                throw new ParseException(
                    state.Previous().Span,
                    "array length must be a positive integer");
            }

            state.ConsumeOrError(TokenKind.RightBracket);
            var spanEnd = state.Previous().Span;
            var span = SourceSpan.Merge(spanStart, spanEnd);
            return new ArraySignature(span, underlyingType, length);
        }

        private static ITypeSignature ParseTypeName(ParserState state)
        {
            state.ConsumeOrError(TokenKind.Identifier, "type name");
            return TypeName.FromToken(state.Previous());
        }
    }
}

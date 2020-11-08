using WireC.AST;
using WireC.AST.TypeSignatures;
using WireC.Common;

namespace WireC.FrontEnd
{
    // TODO: Find a better name for this class.
    public static class TypeSignatureParser
    {
        public static ITypeSignature ParseTypeSignature(ParserState state)
        {
            return ParseTypeName(state);
        }

        private static ITypeSignature ParseTypeName(ParserState state)
        {
            state.ConsumeOrError(TokenKind.Identifier, "type name");
            return TypeName.FromToken(state.Previous());
        }
    }
}

using System.Diagnostics;

using WireC.Common;

namespace WireC.AST.TypeSignatures
{
    public class TypeName : ITypeSignature
    {
        public TypeName(string name, SourceSpan span)
        {
            Name = name;
            Span = span;
        }

        public string Name { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(ITypeSignatureVisitor<T> visitor) => visitor.VisitTypeName(this);

        public static TypeName FromToken(Token token)
        {
            Debug.Assert(token.Kind == TokenKind.Identifier);
            return new TypeName(token.Lexeme, token.Span);
        }

        public override string ToString() => Name;
    }
}
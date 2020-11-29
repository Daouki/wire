using WireC.Common;

namespace WireC.AST.TypeSignatures
{
    public class PointerSignature : ITypeSignature
    {
        public PointerSignature(SourceSpan span, ITypeSignature underlyingType)
        {
            Span = span;
            UnderlyingType = underlyingType;
        }

        public ITypeSignature UnderlyingType { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(ITypeSignatureVisitor<T> visitor) => visitor.VisitPointerSignature(this);
    }
}

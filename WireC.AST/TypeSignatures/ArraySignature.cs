using WireC.Common;

namespace WireC.AST.TypeSignatures
{
    public class ArraySignature : ITypeSignature
    {
        public ArraySignature(SourceSpan span, ITypeSignature underlyingType, int length)
        {
            Span = span;
            UnderlyingType = underlyingType;
            Length = length;
        }

        public ITypeSignature UnderlyingType { get; }
        public int Length { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(ITypeSignatureVisitor<T> visitor) => visitor.VisitArraySignature(this);
    }
}
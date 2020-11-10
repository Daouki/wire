using WireC.Common;

namespace WireC.AST
{
    public interface ITypeSignature
    {
        public SourceSpan Span { get; }

        T Accept<T>(ITypeSignatureVisitor<T> visitor);
    }
}
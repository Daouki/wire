using WireC.Common;

namespace WireC.AST
{
    public interface IExpression : INode
    {
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor);
    }
}

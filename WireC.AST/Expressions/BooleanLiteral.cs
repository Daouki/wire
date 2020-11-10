using WireC.Common;

namespace WireC.AST.Expressions
{
    public class BooleanLiteral : IExpression
    {
        public BooleanLiteral(int nodeId, SourceSpan span, bool value)
        {
            NodeId = nodeId;
            Span = span;
            Value = value;
        }

        public bool Value { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBooleanLiteral(this);
    }
}
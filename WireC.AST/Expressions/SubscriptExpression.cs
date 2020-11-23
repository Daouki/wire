using WireC.Common;

namespace WireC.AST.Expressions
{
    public class SubscriptExpression : IExpression
    {
        public SubscriptExpression(
            int nodeId,
            SourceSpan span,
            IExpression operand,
            IExpression index)
        {
            NodeId = nodeId;
            Span = span;
            Operand = operand;
            Index = index;
        }

        public IExpression Operand { get; }
        public IExpression Index { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSubscriptExpression(this);
    }
}
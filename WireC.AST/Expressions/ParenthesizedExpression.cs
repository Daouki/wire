using WireC.Common;

namespace WireC.AST.Expressions
{
    public class ParenthesizedExpression : IExpression
    {
        public ParenthesizedExpression(int nodeId, SourceSpan span, IExpression expression)
        {
            NodeId = nodeId;
            Span = span;
            Expression = expression;
        }

        public IExpression Expression { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) =>
            visitor.VisitParenthesizedExpression(this);
    }
}
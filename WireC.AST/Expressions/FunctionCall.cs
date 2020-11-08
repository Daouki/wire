using WireC.Common;

namespace WireC.AST.Expressions
{
    public class FunctionCall : IExpression
    {
        public int NodeId { get; }
        public IExpression Callee { get; }
        public SourceSpan Span { get; }

        public FunctionCall(int nodeId, SourceSpan span, IExpression callee)
        {
            NodeId = nodeId;
            Callee = callee;
            Span = span;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitFunctionCall(this);
    }
}

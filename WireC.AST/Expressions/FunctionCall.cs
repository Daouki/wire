using WireC.Common;

namespace WireC.AST.Expressions
{
    public class FunctionCall : IExpression
    {
        public FunctionCall(int nodeId, SourceSpan span, IExpression callee)
        {
            NodeId = nodeId;
            Callee = callee;
            Span = span;
        }

        public IExpression Callee { get; }
        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitFunctionCall(this);
    }
}
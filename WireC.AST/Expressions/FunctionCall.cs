using WireC.Common;

namespace WireC.AST.Expressions
{
    public class FunctionCall : IExpression
    {
        public IExpression Callee { get; }
        public SourceSpan Span { get; }

        public FunctionCall(SourceSpan span, IExpression callee)
        {
            Callee = callee;
            Span = span;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitFunctionCall(this);
    }
}

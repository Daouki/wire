using System.Collections.Generic;

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class FunctionCall : IExpression
    {
        public FunctionCall(
            int nodeId,
            SourceSpan span,
            IExpression callee,
            List<IExpression> arguments)
        {
            NodeId = nodeId;
            Callee = callee;
            Span = span;
            Arguments = arguments;
        }

        public IExpression Callee { get; }
        public List<IExpression> Arguments { get; }
        public int Arity => Arguments.Count;

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitFunctionCall(this);
    }
}

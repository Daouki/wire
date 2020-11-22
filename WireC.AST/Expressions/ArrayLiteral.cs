using System.Collections.Generic;

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class ArrayLiteral : IExpression
    {
        public ArrayLiteral(int nodeId, SourceSpan span, List<IExpression> elements)
        {
            NodeId = nodeId;
            Span = span;
            Elements = elements;
        }

        public List<IExpression> Elements { get; }
        public int Length => Elements.Count;
        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitArrayLiteral(this);
    }
}
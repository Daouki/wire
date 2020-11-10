using WireC.Common;

namespace WireC.AST.Expressions
{
    public class IntegerLiteral : IExpression
    {
        public IntegerLiteral(int nodeId, Token token)
        {
            NodeId = nodeId;
            Span = token.Span;
            Value = long.Parse(token.Lexeme);
        }

        public int NodeId { get; }
        public SourceSpan Span { get; }
        public long Value { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIntegerLiteral(this);
    }
}

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class IdentifierLiteral : IExpression
    {
        public IdentifierLiteral(int nodeId, Token token)
        {
            NodeId = nodeId;
            Span = token.Span;
            Name = token.Lexeme;
        }

        public int NodeId { get; }
        public SourceSpan Span { get; }
        public string Name { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIdentifier(this);
    }
}

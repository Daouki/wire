using WireC.Common;

namespace WireC.AST.Expressions
{
    public class Identifier : IExpression
    {
        public Identifier(int nodeId, Token token)
        {
            NodeId = nodeId;
            Span = token.Span;
            Token = token;
            Name = token.Lexeme;
        }

        public Token Token { get; }

        public string Name { get; }

        public int NodeId { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIdentifier(this);
    }
}
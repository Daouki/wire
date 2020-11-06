using WireC.Common;

namespace WireC.AST.Expressions
{
    public class IntegerLiteral : IExpression
    {
        public IntegerLiteral(Token token)
        {
            Span = token.Span;
            Token = token;
        }

        public Token Token { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIntegerLiteral(this);
    }
}
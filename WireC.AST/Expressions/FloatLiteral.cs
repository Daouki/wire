using System.Globalization;

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class FloatLiteral : IExpression
    {
        public FloatLiteral(int nodeId, Token token)
        {
            NodeId = nodeId;
            Span = token.Span;
            Value = double.Parse(token.Lexeme, CultureInfo.InvariantCulture);
        }

        public int NodeId { get; }
        public SourceSpan Span { get; }
        public double Value { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitFloatLiteral(this);
    }
}

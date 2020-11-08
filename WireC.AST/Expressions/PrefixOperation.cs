using System;

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class PrefixOperation : IExpression
    {
        public PrefixOperation(int nodeId, PrefixOperator @operator, IExpression operand)
        {
            NodeId = nodeId;
            Operator = @operator;
            Operand = operand;
            Span = SourceSpan.Merge(Operator.Span, Operand.Span);
        }

        public int NodeId { get; }

        /// <summary>
        /// Prefix unary operation operator.
        /// </summary>
        public PrefixOperator Operator;

        /// <summary>
        /// Operand of the prefix unary operation.
        /// </summary>
        public IExpression Operand;

        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPrefixOperation(this);
    }

    public class PrefixOperator
    {
        public PrefixOperator(PrefixOperatorKind kind, SourceSpan span)
        {
            Kind = kind;
            Span = span;
        }

        public static PrefixOperator FromToken(Token token)
        {
            return new PrefixOperator(
                token.Kind switch
                {
                    TokenKind.Plus => PrefixOperatorKind.Identity,
                    TokenKind.Minus => PrefixOperatorKind.Negate,
                    _ => throw new ArgumentException(nameof(token)),
                },
                token.Span
            );
        }

        public PrefixOperatorKind Kind { get; }
        public SourceSpan Span { get; }
    }

    public enum PrefixOperatorKind
    {
        Identity,
        Negate,
    }
}

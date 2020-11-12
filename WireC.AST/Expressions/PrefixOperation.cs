using System;

using WireC.Common;

namespace WireC.AST.Expressions
{
    public class PrefixOperation : IExpression
    {
        /// <summary>
        /// Operand of the prefix unary operation.
        /// </summary>
        public IExpression Operand;

        /// <summary>
        /// Prefix unary operation operator.
        /// </summary>
        public Spanned<PrefixOperator> Operator;

        public PrefixOperation(int nodeId, Token @operator, IExpression operand)
        {
            NodeId = nodeId;
            Span = SourceSpan.Merge(@operator.Span, operand.Span);
            Operator = new Spanned<PrefixOperator>(
                @operator.Kind switch
                {
                    TokenKind.Bang => PrefixOperator.Not,
                    TokenKind.Plus => PrefixOperator.Identity,
                    TokenKind.Minus => PrefixOperator.Negate,
                    _ => throw new ArgumentException(nameof(@operator)),
                },
                Span
            );
            Operand = operand;
        }

        public int NodeId { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPrefixOperation(this);
    }

    public enum PrefixOperator
    {
        Identity, // The "+" operator that does nothing.
        Negate, // The "-" operator for negation.
        Not, // The "!" operator for logical inversion.
    }
}
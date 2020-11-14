using System;
using System.ComponentModel;

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
                    TokenKind.Tilde => PrefixOperator.BinaryNot,
                    _ => throw new ArgumentException(nameof(@operator)),
                },
                Span);
            Operand = operand;
        }

        public int NodeId { get; }

        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitPrefixOperation(this);
    }

    public enum PrefixOperator
    {
        [Description("\"+\"")]
        Identity, // The "+" operator that does nothing.

        [Description("\"-\"")]
        Negate, // The "-" operator for negation.

        [Description("\"!\"")]
        Not, // The "!" operator for logical inversion.

        [Description("\"~\"")]
        BinaryNot, // The "~" operator for bitwise inversion.
    }

    public static class PrefixOperatorExtensions
    {
        public static string GetDescription(this PrefixOperator op)
        {
            var type = op.GetType();
            var memberInfo = type.GetMember(op.ToString());
            if (memberInfo.Length <= 0) return op.ToString();
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0
                ? ((DescriptionAttribute) attrs[0]).Description
                : op.ToString();
        }
    }
}
using System;

using WireC.Common;

namespace WireC.AST.Expressions
{
    /// <summary>
    /// An binary (two operands) infix operation expression.
    /// </summary>
    public class InfixOperation : IExpression
    {
        public InfixOperation(
            int nodeId,
            InfixOperator @operator,
            IExpression leftOperand,
            IExpression rightOperand)
        {
            NodeId = nodeId;
            Span = SourceSpan.Merge(leftOperand.Span, rightOperand.Span);
            Operator = @operator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public InfixOperator Operator { get; }
        public IExpression LeftOperand { get; }
        public IExpression RightOperand { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitInfixOperation(this);
    }

    public class InfixOperator
    {
        public InfixOperator(InfixOperatorKind kind, SourceSpan span)
        {
            Kind = kind;
            Span = span;
        }

        public InfixOperatorKind Kind { get; }
        public SourceSpan Span { get; }

        public static InfixOperator FromToken(Token token)
        {
            return new InfixOperator(
                token.Kind switch
                {
                    TokenKind.Asterisk => InfixOperatorKind.Multiply,
                    TokenKind.Minus => InfixOperatorKind.Subtract,
                    TokenKind.Plus => InfixOperatorKind.Add,
                    TokenKind.Slash => InfixOperatorKind.Divide,
                    _ => throw new ArgumentException(nameof(token)),
                },
                token.Span
            );
        }
    }

    public enum InfixOperatorKind
    {
        Add,
        Divide,
        Multiply,
        Subtract,
    }
}
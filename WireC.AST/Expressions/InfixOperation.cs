﻿using System;

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
            Token @operator,
            IExpression leftOperand,
            IExpression rightOperand)
        {
            NodeId = nodeId;
            Span = SourceSpan.Merge(leftOperand.Span, rightOperand.Span);
            Operator = new Spanned<InfixOperator>(
                @operator.Kind switch
                {
                    TokenKind.Ampersand => InfixOperator.BinaryAnd,
                    TokenKind.AmpersandAmpersand => InfixOperator.LogicalAnd,
                    TokenKind.Asterisk => InfixOperator.Multiply,
                    TokenKind.Caret => InfixOperator.BinaryXor,
                    TokenKind.Equal => InfixOperator.Equal,
                    TokenKind.EqualEqual => InfixOperator.Equal,
                    TokenKind.Greater => InfixOperator.Greater,
                    TokenKind.GreaterEqual => InfixOperator.GreaterOrEqual,
                    TokenKind.GreaterGreater => InfixOperator.ShiftRight,
                    TokenKind.Less => InfixOperator.Less,
                    TokenKind.LessEqual => InfixOperator.LessOrEqual,
                    TokenKind.LessGreater => InfixOperator.NotEqual,
                    TokenKind.LessLess => InfixOperator.ShiftLeft,
                    TokenKind.Minus => InfixOperator.Subtract,
                    TokenKind.Percent => InfixOperator.Modulo,
                    TokenKind.Pipe => InfixOperator.BinaryOr,
                    TokenKind.PipePipe => InfixOperator.LogicalOr,
                    TokenKind.Plus => InfixOperator.Add,
                    TokenKind.Slash => InfixOperator.Divide,
                    _ => throw new ArgumentException(
                        $"Cannot create an infix operator from a token {@operator.Kind}"),
                },
                @operator.Span);
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public Spanned<InfixOperator> Operator { get; }
        public IExpression LeftOperand { get; }
        public IExpression RightOperand { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitInfixOperation(this);
    }

    public enum InfixOperator
    {
        Add,
        BinaryAnd,
        BinaryOr,
        BinaryXor,
        Divide,
        Equal,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        LogicalAnd,
        LogicalOr,
        Modulo,
        Multiply,
        NotEqual,
        ShiftLeft,
        ShiftRight,
        Subtract,
    }
}

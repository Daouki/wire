﻿using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class IntegerType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperator @operator) => @operator switch
        {
            PrefixOperator.BinaryNot => new IntegerType(),
            PrefixOperator.Identity => new IntegerType(),
            PrefixOperator.Negate => new IntegerType(),
            _ => null,
        };

        public IType GetInfixOperationResultType(InfixOperator @operator) => @operator switch
        {
            InfixOperator.Add => new IntegerType(),
            InfixOperator.BinaryAnd => new IntegerType(),
            InfixOperator.BinaryOr => new IntegerType(),
            InfixOperator.BinaryXor => new IntegerType(),
            InfixOperator.Divide => new IntegerType(),
            InfixOperator.Equal => new BooleanType(),
            InfixOperator.Greater => new BooleanType(),
            InfixOperator.GreaterOrEqual => new BooleanType(),
            InfixOperator.Less => new BooleanType(),
            InfixOperator.LessOrEqual => new BooleanType(),
            InfixOperator.Modulo => new IntegerType(),
            InfixOperator.Multiply => new IntegerType(),
            InfixOperator.NotEqual => new BooleanType(),
            InfixOperator.ShiftLeft => new IntegerType(),
            InfixOperator.ShiftRight => new IntegerType(),
            InfixOperator.Subtract => new IntegerType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is IntegerType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitIntegerType(this);

        public override string ToString() => "i64";
    }
}

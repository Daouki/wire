using System;

using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class IntegerType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind operationKind) =>
            operationKind switch
            {
                PrefixOperatorKind.Identity => new IntegerType(),
                PrefixOperatorKind.Negate => new IntegerType(),
                _ => throw new ArgumentException(nameof(operationKind)),
            };

        public bool IsSame(IType other) => other is IntegerType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitIntegerType(this);
    }
}

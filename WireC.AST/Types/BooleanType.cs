using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class BooleanType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperator @operator) => @operator switch
        {
            PrefixOperator.Not => new BooleanType(),
            _ => null,
        };

        public IType GetInfixOperationResultType(InfixOperator @operator) => @operator switch
        {
            InfixOperator.BinaryAnd => new BooleanType(),
            InfixOperator.BinaryOr => new BooleanType(),
            InfixOperator.BinaryXor => new BooleanType(),
            InfixOperator.Equal => new BooleanType(),
            InfixOperator.LogicalAnd => new BooleanType(),
            InfixOperator.LogicalOr => new BooleanType(),
            InfixOperator.NotEqual => new BooleanType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is BooleanType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitBooleanType(this);

        public override string ToString() => "bool";
    }
}

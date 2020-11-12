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
            InfixOperator.Equal => new BooleanType(),
            InfixOperator.NotEqual => new BooleanType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is BooleanType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitBooleanType(this);

        public override string ToString() => "bool";
    }
}

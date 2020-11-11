using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class BooleanType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind @operator) => @operator switch
        {
            PrefixOperatorKind.Not => new BooleanType(),
            _ => null,
        };

        public IType GetInfixOperationResultType(InfixOperatorKind @operator) => @operator switch
        {
            InfixOperatorKind.Equal => new BooleanType(),
            InfixOperatorKind.NotEqual => new BooleanType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is BooleanType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitBooleanType(this);

        public override string ToString() => "bool";
    }
}

using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class BooleanType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind @operator) => null;

        public IType GetInfixOperationResultType(InfixOperatorKind @operator) => null;

        public bool IsSame(IType other) => other is BooleanType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitBooleanType(this);

        public override string ToString() => "bool";
    }
}
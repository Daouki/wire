using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class PointerType : IType
    {
        public PointerType(IType underlyingType)
        {
            UnderlyingType = underlyingType;
        }

        public IType UnderlyingType { get; }

        public IType GetPrefixOperationResultType(PrefixOperator @operator) => null;

        public IType GetInfixOperationResultType(InfixOperator @operator) => null;

        public bool IsSame(IType other) => other is PointerType otherPointerType &&
            UnderlyingType.IsSame(otherPointerType.UnderlyingType);

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitPointerType(this);

        public override string ToString() => $"^{UnderlyingType}";
    }
}

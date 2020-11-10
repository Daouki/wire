using WireC.AST.Expressions;

namespace WireC.AST
{
    public interface IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind @operator);
        public IType GetInfixOperationResultType(InfixOperatorKind @operator);

        bool IsSame(IType other);

        T Accept<T>(ITypeVisitor<T> visitor);
    }
}
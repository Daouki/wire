using WireC.AST.Expressions;

namespace WireC.AST
{
    public interface IType
    {
        public IType GetPrefixOperationResultType(PrefixOperator @operator);
        public IType GetInfixOperationResultType(InfixOperator @operator);

        bool IsSame(IType other);

        T Accept<T>(ITypeVisitor<T> visitor);
    }
}

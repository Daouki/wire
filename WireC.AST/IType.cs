using WireC.AST.Expressions;

namespace WireC.AST
{
    public interface IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind operationKind);

        bool IsSame(IType other);

        T Accept<T>(ITypeVisitor<T> visitor);
    }
}
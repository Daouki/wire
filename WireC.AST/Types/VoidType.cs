using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class VoidType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind operationKind) => null;

        public bool IsSame(IType other) => other is VoidType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitVoidType(this);

        public override string ToString() => "void";
    }
}
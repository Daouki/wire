using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class FunctionType : IType
    {
        public FunctionType(IType returnType)
        {
            ReturnType = returnType;
        }

        public IType ReturnType { get; }

        public IType GetPrefixOperationResultType(PrefixOperatorKind operationKind) => null;

        public bool IsSame(IType other)
        {
            if (other is not FunctionType) return false;
            var otherFunctionType = (FunctionType) other;
            return ReturnType.IsSame(otherFunctionType);
        }

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitFunctionType(this);
    }
}

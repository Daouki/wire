using System.Collections.Generic;
using System.Diagnostics;

using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class FunctionType : IType
    {
        public FunctionType(List<IType> parameterTypes, IType returnType)
        {
            Debug.Assert(parameterTypes != null);
            ParameterTypes = parameterTypes;
            ReturnType = returnType;
        }

        public List<IType> ParameterTypes { get; }
        public IType ReturnType { get; }
        public int Arity => ParameterTypes.Count;

        public IType GetPrefixOperationResultType(PrefixOperator operationKind) => null;

        public IType GetInfixOperationResultType(InfixOperator @operator) => null;

        public bool IsSame(IType other)
        {
            if (other is not FunctionType) return false;
            var otherFunctionType = (FunctionType) other;
            return ReturnType.IsSame(otherFunctionType);
        }

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitFunctionType(this);

        public override string ToString() => $"fn(): {ReturnType}";
    }
}
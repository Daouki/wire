using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class ArrayType : IType
    {
        public ArrayType(IType underlyingType, IExpression length)
        {
            UnderlyingType = underlyingType;
            Length = length;
        }

        public IType UnderlyingType { get; }
        public IExpression Length { get; }

        public IType GetPrefixOperationResultType(PrefixOperator @operator) => null;

        public IType GetInfixOperationResultType(InfixOperator @operator) => null;

        public bool IsSame(IType other) =>
            other is ArrayType otherArray && Length == otherArray.Length;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitArrayType(this);

        public override string ToString() => $"[{UnderlyingType}; {Length}]";
    }
}

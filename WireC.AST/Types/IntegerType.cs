namespace WireC.AST.Types
{
    public class IntegerType : IType
    {
        public bool IsSame(IType other) => other is IntegerType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitIntegerType(this);
    }
}

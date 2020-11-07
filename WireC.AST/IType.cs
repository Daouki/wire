namespace WireC.AST
{
    public interface IType
    {
        bool IsSame(IType other);

        T Accept<T>(ITypeVisitor<T> visitor);
    }
}

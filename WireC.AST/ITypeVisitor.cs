using WireC.AST.Types;

namespace WireC.AST
{
    public interface ITypeVisitor<T>
    {
        T VisitIntegerType(IntegerType integer);
        T VisitFunctionType(FunctionType functionType);
        T VisitVoidType(VoidType voidType);
    }
}
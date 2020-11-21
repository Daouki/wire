using WireC.AST.Types;

namespace WireC.AST
{
    public interface ITypeVisitor<out T>
    {
        T VisitIntegerType(IntegerType integer);
        T VisitFunctionType(FunctionType functionType);
        T VisitVoidType(VoidType voidType);
        T VisitBooleanType(BooleanType booleanType);
        T VisitFloatType(FloatType floatType);
        T VisitArrayType(ArrayType arrayType);
    }
}
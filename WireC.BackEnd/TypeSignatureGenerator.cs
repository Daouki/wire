using System;
using System.Text;

using WireC.AST;
using WireC.AST.Types;

namespace WireC.BackEnd
{
    public class TypeSignatureGenerator : ITypeVisitor<string>
    {
        private TypeSignatureGenerator()
        {
        }

        public string VisitIntegerType(IntegerType integer) => "int64_t";

        public string VisitFunctionType(FunctionType functionType) =>
            throw new NotImplementedException();

        public string VisitVoidType(VoidType voidType) => "void";
        public string VisitBooleanType(BooleanType booleanType) => "bool";
        public string VisitFloatType(FloatType floatType) => "double";

        public string VisitArrayType(ArrayType arrayType) => new StringBuilder()
            .Append("std::array<")
            .Append(GenerateTypeSignature(arrayType.UnderlyingType))
            .Append(", ")
            .Append(ExpressionCodeGenerator.GenerateExpressionCode(arrayType.Length))
            .Append('>')
            .ToString();

        public static string GenerateTypeSignature(IType type)
        {
            var self = new TypeSignatureGenerator();
            return type.Accept(self);
        }
    }
}

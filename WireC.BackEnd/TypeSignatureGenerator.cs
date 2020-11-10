using System;

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

        public static string GenerateTypeSignature(IType type)
        {
            var self = new TypeSignatureGenerator();
            return type.Accept(self);
        }
    }
}
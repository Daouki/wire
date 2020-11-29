using WireC.AST.TypeSignatures;

namespace WireC.AST
{
    public interface ITypeSignatureVisitor<out T>
    {
        T VisitTypeName(TypeName typeName);
        T VisitArraySignature(ArraySignature arraySignature);
        T VisitPointerSignature(PointerSignature pointerSignature);
    }
}

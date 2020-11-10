using System.Collections.Generic;

using WireC.AST;
using WireC.AST.Types;
using WireC.AST.TypeSignatures;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class TypeSignatureParser : ITypeSignatureVisitor<IType>
    {
        private static readonly Dictionary<string, IType> _primitiveTypes =
            new Dictionary<string, IType>
            {
                {"i64", new IntegerType()},
            };

        private readonly Context _context;

        private TypeSignatureParser(Context context)
        {
            _context = context;
        }

        public IType VisitTypeName(TypeName typeName)
        {
            if (_primitiveTypes.TryGetValue(typeName.Name, out var primitiveType))
                return primitiveType;
            _context.Error(
                typeName.Span,
                $"type \"{typeName.Name}\" was not defined in the current scope"
            );
            return null;
        }

        public static IType ParseTypeSignature(Context context, ITypeSignature typeSignature)
        {
            var self = new TypeSignatureParser(context);
            return typeSignature.Accept(self);
        }
    }
}
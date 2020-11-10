using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class IntegerType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperatorKind @operator) => @operator switch
        {
            PrefixOperatorKind.Identity => new IntegerType(),
            PrefixOperatorKind.Negate => new IntegerType(),
            _ => null,
        };

        public IType GetInfixOperationResultType(InfixOperatorKind @operator) => @operator switch
        {
            InfixOperatorKind.Add => new IntegerType(),
            InfixOperatorKind.Divide => new IntegerType(),
            InfixOperatorKind.Multiply => new IntegerType(),
            InfixOperatorKind.Subtract => new IntegerType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is IntegerType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitIntegerType(this);

        public override string ToString() => "i64";
    }
}

using WireC.AST.Expressions;

namespace WireC.AST.Types
{
    public class FloatType : IType
    {
        public IType GetPrefixOperationResultType(PrefixOperator @operator) => @operator switch
        {
            PrefixOperator.Identity => new FloatType(),
            PrefixOperator.Negate => new FloatType(),
            _ => null,
        };

        public IType GetInfixOperationResultType(InfixOperator @operator) => @operator switch
        {
            InfixOperator.Add => new FloatType(),
            InfixOperator.Divide => new FloatType(),
            InfixOperator.Equal => new BooleanType(),
            InfixOperator.Greater => new BooleanType(),
            InfixOperator.GreaterOrEqual => new BooleanType(),
            InfixOperator.Less => new BooleanType(),
            InfixOperator.LessOrEqual => new BooleanType(),
            InfixOperator.Multiply => new FloatType(),
            InfixOperator.NotEqual => new BooleanType(),
            InfixOperator.Subtract => new FloatType(),
            _ => null,
        };

        public bool IsSame(IType other) => other is FloatType;

        public T Accept<T>(ITypeVisitor<T> visitor) => visitor.VisitFloatType(this);

        public override string ToString() => "f64";
    }
}

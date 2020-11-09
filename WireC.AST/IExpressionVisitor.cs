using WireC.AST.Expressions;

namespace WireC.AST
{
    public interface IExpressionVisitor<out T>
    {
        T VisitIdentifier(Identifier identifier);
        T VisitIntegerLiteral(IntegerLiteral integer);
        T VisitFunctionCall(FunctionCall functionCall);
        T VisitPrefixOperation(PrefixOperation prefixOperation);
        T VisitInfixOperation(InfixOperation infixOperation);
    }
}

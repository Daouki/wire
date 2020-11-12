using WireC.AST.Expressions;

namespace WireC.AST
{
    public interface IExpressionVisitor<out T>
    {
        T VisitIdentifier(IdentifierLiteral identifierLiteral);
        T VisitIntegerLiteral(IntegerLiteral integer);
        T VisitFunctionCall(FunctionCall functionCall);
        T VisitPrefixOperation(PrefixOperation prefixOperation);
        T VisitInfixOperation(InfixOperation infixOperation);
        T VisitBooleanLiteral(BooleanLiteral booleanLiteral);
        T VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression);
    }
}
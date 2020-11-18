using WireC.AST;
using WireC.AST.Expressions;
using WireC.AST.Types;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class Typer : IExpressionVisitor<IType>
    {
        private readonly Context _context;

        private readonly Scope _environment;

        private Typer(Context context, Scope environment)
        {
            _context = context;
            _environment = environment;
        }

        public IType VisitIdentifier(IdentifierLiteral identifierLiteral)
        {
            var symbol = _environment.GetSymbol(identifierLiteral.Name);
            if (symbol != null) return symbol.Type;
            _context.Error(
                identifierLiteral.Span,
                $"symbol \"{identifierLiteral.Name}\" was not declared in the current scope");
            return null;
        }

        public IType VisitIntegerLiteral(IntegerLiteral integer) => new IntegerType();

        public IType VisitFunctionCall(FunctionCall functionCall)
        {
            var calleeType = GetExpressionType(functionCall.Callee);
            if (calleeType is FunctionType calleeFunctionType) return calleeFunctionType.ReturnType;

            _context.Error(
                functionCall.Callee.Span,
                "cannot call a non-callable type \"{calleType}\"");
            return null;
        }

        public IType VisitPrefixOperation(PrefixOperation prefixOperation) =>
            GetExpressionType(prefixOperation.Operand)
                .GetPrefixOperationResultType(prefixOperation.Operator.Node);

        public IType VisitInfixOperation(InfixOperation infixOperation)
        {
            var leftOperandType = GetExpressionType(infixOperation.LeftOperand);
            var rightOperandType = GetExpressionType(infixOperation.RightOperand);
            if (leftOperandType != null && leftOperandType.IsSame(rightOperandType))
                return leftOperandType.GetInfixOperationResultType(infixOperation.Operator.Node);
            _context.Error(
                infixOperation.Operator.Span,
                "type mismatch between operands of infix operation; " +
                $"left is \"{leftOperandType}\", but right is \"{rightOperandType}\"");
            return null;
        }

        public IType VisitBooleanLiteral(BooleanLiteral booleanLiteral) => new BooleanType();

        public IType VisitParenthesizedExpression(
            ParenthesizedExpression parenthesizedExpression) =>
            GetExpressionType(parenthesizedExpression.Expression);

        public IType VisitFloatLiteral(FloatLiteral floatLiteral) => new FloatType();

        public static IType GetExpressionType(
            Context context,
            Scope environment,
            IExpression expression)
        {
            var self = new Typer(context, environment);
            return expression.Accept(self);
        }

        private IType GetExpressionType(IExpression expression) => expression.Accept(this);
    }
}

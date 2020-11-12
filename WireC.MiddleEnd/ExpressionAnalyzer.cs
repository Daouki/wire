using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class ExpressionAnalyzer : IExpressionVisitor<bool>
    {
        private readonly Context _context;
        private readonly Scope _environment;

        private ExpressionAnalyzer(Context context, Scope environment)
        {
            _context = context;
            _environment = environment;
        }

        public bool VisitIdentifier(IdentifierLiteral identifierLiteral)
        {
            if (!_environment.IsSymbolDefined(identifierLiteral.Name))
            {
                _context.Error(
                    identifierLiteral.Span,
                    $"symbol \"{identifierLiteral.Name}\" was not defined in the current scope"
                );
                return false;
            }

            return true;
        }

        public bool VisitIntegerLiteral(IntegerLiteral integer) => true;

        public bool VisitFunctionCall(FunctionCall functionCall)
        {
            var calleeType = Typer.GetExpressionType(_context, _environment, functionCall.Callee);
            return calleeType != null;
        }

        public bool VisitPrefixOperation(PrefixOperation prefixOperation)
        {
            if (!IsExpressionValid(prefixOperation.Operand)) return false;

            var operandType = Typer.GetExpressionType(
                _context,
                _environment,
                prefixOperation.Operand
            );
            if (operandType == null ||
                operandType.GetPrefixOperationResultType(prefixOperation.Operator.Node) != null)
                return true;

            _context.Error(
                prefixOperation.Span,
                "cannot apply unary operator " +
                $"{prefixOperation.Operator.Node.GetDescription()} to type \"{operandType}\""
            );
            return false;
        }

        public bool VisitInfixOperation(InfixOperation infixOperation)
        {
            if (!(IsExpressionValid(infixOperation.LeftOperand) &&
                IsExpressionValid(infixOperation.RightOperand)))
                return false;

            if (infixOperation.LeftOperand is InfixOperation child &&
                infixOperation.Operator.Node != child.Operator.Node)
            {
                _context.Error(
                    infixOperation.Operator.Span,
                    "operator precedence is not supported; parentheses required"
                );
            }

            var leftOperandType = Typer.GetExpressionType(
                _context,
                _environment,
                infixOperation.LeftOperand
            );
            var rightOperandType = Typer.GetExpressionType(
                _context,
                _environment,
                infixOperation.RightOperand
            );

            if (leftOperandType != null &&
                leftOperandType.GetInfixOperationResultType(infixOperation.Operator.Node) == null)
            {
                _context.Error(
                    infixOperation.Operator.Span,
                    $"infix operation \"{infixOperation.Operator.Node}\" " +
                    $"is not defined for types \"{leftOperandType}\" and \"{rightOperandType}\""
                );
                return false;
            }

            if (leftOperandType != null && leftOperandType.IsSame(rightOperandType)) return true;

            _context.Error(
                infixOperation.Operator.Span,
                "type mismatch between operands of a infix expression; " +
                $"left is \"{leftOperandType}\", but right is \"{rightOperandType}\""
            );
            return false;
        }

        public bool VisitBooleanLiteral(BooleanLiteral booleanLiteral) => true;

        public bool VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression) =>
            IsExpressionValid(parenthesizedExpression.Expression);

        public static bool IsExpressionValid(
            Context context,
            Scope environment,
            IExpression expression)
        {
            var self = new ExpressionAnalyzer(context, environment);
            return expression.Accept(self);
        }

        private bool IsExpressionValid(IExpression expression) => expression.Accept(this);
    }
}

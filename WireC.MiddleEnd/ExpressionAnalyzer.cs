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

        public bool VisitIdentifier(Identifier identifier)
        {
            if (!_environment.IsSymbolDefined(identifier.Name))
            {
                _context.Error(
                    identifier.Span,
                    $"symbol \"{identifier.Name}\" was not defined in the current scope"
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

        public bool VisitPrefixOperation(PrefixOperation prefixOperation) =>
            IsExpressionValid(prefixOperation.Operand);

        public bool VisitInfixOperation(InfixOperation infixOperation)
        {
            if (!(IsExpressionValid(infixOperation.LeftOperand) &&
                IsExpressionValid(infixOperation.RightOperand)))
                return false;

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

            if (leftOperandType != null && leftOperandType.IsSame(rightOperandType)) return true;

            _context.Error(
                infixOperation.Operator.Span,
                "type mismatch between operands of a infix expression; " +
                $"left is \"{leftOperandType}\", but right is \"{rightOperandType}\""
            );
            return false;
        }

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
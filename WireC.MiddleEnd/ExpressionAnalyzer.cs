using System;
using System.Linq;

using WireC.AST;
using WireC.AST.Expressions;
using WireC.AST.Types;
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
                    $"symbol \"{identifierLiteral.Name}\" was not defined in the current scope");
                return false;
            }

            return true;
        }

        public bool VisitIntegerLiteral(IntegerLiteral integer) => true;

        public bool VisitFunctionCall(FunctionCall functionCall)
        {
            var calleeType = Typer.GetExpressionType(_context, _environment, functionCall.Callee);
            if (calleeType == null) return false;
            if (!(calleeType is FunctionType))
            {
                _context.Error(
                    functionCall.Span,
                    $"tried to call a non-callable type \"{calleeType}\"");
                return false;
            }

            var calleeFunctionType = (FunctionType) calleeType;
            if (functionCall.Arity != calleeFunctionType.Arity)
            {
                var s = calleeFunctionType.Arity == 1 ? "" : "s";
                var wasOrWere = functionCall.Arity == 1 ? "was" : "were";
                _context.Error(
                    functionCall.Callee.Span,
                    $"expected {calleeFunctionType.Arity} argument{s}, " +
                    $"but {functionCall.Arity} {wasOrWere} given");
            }

            var argumentsToCheck = Math.Min(functionCall.Arity, calleeFunctionType.Arity);
            var allArgumentsHaveCorrectType = true;
            for (var i = 0; i < argumentsToCheck; i++)
            {
                var argument = functionCall.Arguments[i];
                var argumentType = Typer.GetExpressionType(_context, _environment, argument);
                var parameterType = calleeFunctionType.ParameterTypes[i];
                if (parameterType.IsSame(argumentType)) continue;
                _context.Error(
                    argument.Span,
                    "type mismatch between argument and parameter; " +
                    $"expected \"{parameterType}\", but found \"{argumentType}\"");
                allArgumentsHaveCorrectType = false;
            }

            return allArgumentsHaveCorrectType;
        }

        public bool VisitPrefixOperation(PrefixOperation prefixOperation)
        {
            if (!IsExpressionValid(prefixOperation.Operand)) return false;

            var operandType = Typer.GetExpressionType(
                _context,
                _environment,
                prefixOperation.Operand);
            if (operandType == null ||
                operandType.GetPrefixOperationResultType(prefixOperation.Operator.Node) != null)
                return true;

            _context.Error(
                prefixOperation.Span,
                "cannot apply unary operator " +
                $"{prefixOperation.Operator.Node.GetDescription()} to type \"{operandType}\"");
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
                    "operator precedence is not supported; parentheses required");
            }

            var leftOperandType = Typer.GetExpressionType(
                _context,
                _environment,
                infixOperation.LeftOperand);
            var rightOperandType = Typer.GetExpressionType(
                _context,
                _environment,
                infixOperation.RightOperand);

            if (leftOperandType != null &&
                leftOperandType.GetInfixOperationResultType(infixOperation.Operator.Node) == null)
            {
                _context.Error(
                    infixOperation.Operator.Span,
                    $"infix operation \"{infixOperation.Operator.Node}\" " +
                    $"is not defined for types \"{leftOperandType}\" and \"{rightOperandType}\"");
                return false;
            }

            if (leftOperandType != null && leftOperandType.IsSame(rightOperandType)) return true;

            _context.Error(
                infixOperation.Operator.Span,
                "type mismatch between operands of a infix expression; " +
                $"left is \"{leftOperandType}\", but right is \"{rightOperandType}\"");
            return false;
        }

        public bool VisitBooleanLiteral(BooleanLiteral booleanLiteral) => true;

        public bool VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression) =>
            IsExpressionValid(parenthesizedExpression.Expression);

        public bool VisitFloatLiteral(FloatLiteral floatLiteral) => true;

        public bool VisitArrayLiteral(ArrayLiteral arrayLiteral)
        {
            var isValid = arrayLiteral.Elements.Aggregate(
                true,
                (current, element) => current & IsExpressionValid(element));
            if (!isValid) return false;

            return Typer.GetExpressionType(_context, _environment, arrayLiteral) != null;
        }

        public bool VisitSubscriptExpression(SubscriptExpression subscriptExpression)
        {
            if (!IsExpressionValid(subscriptExpression.Operand) ||
                !IsExpressionValid(subscriptExpression.Index))
                return false;

            var operandType = Typer.GetExpressionType(
                _context,
                _environment,
                subscriptExpression.Operand);
            if (operandType == null) return false;
            if (!(operandType is ArrayType))
            {
                _context.Error(
                    subscriptExpression.Span,
                    $"cannot index into a non-array type \"{operandType}\"");
                return false;
            }

            var indexType = Typer.GetExpressionType(
                _context,
                _environment,
                subscriptExpression.Index);
            if (indexType == null) return false;
            if (!(indexType is IntegerType))
            {
                _context.Error(
                    subscriptExpression.Span,
                    $"type \"{operandType}\" cannot be indexed by type \"{indexType}\"");
                return false;
            }

            return true;
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

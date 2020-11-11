using System;

using WireC.AST;
using WireC.AST.Expressions;

namespace WireC.BackEnd
{
    public class ExpressionCodeGenerator : IExpressionVisitor<string>
    {
        private ExpressionCodeGenerator()
        {
        }

        public string VisitIdentifier(IdentifierLiteral identifierLiteral) =>
            identifierLiteral.Name;

        public string VisitIntegerLiteral(IntegerLiteral integer) => integer.Value.ToString();

        public string VisitFunctionCall(FunctionCall functionCall)
        {
            var callee = GenerateExpression(functionCall.Callee);
            return $"{callee}()";
        }

        public string VisitPrefixOperation(PrefixOperation prefixOperation)
        {
            var @operator = GeneratePrefixOperator(prefixOperation.Operator);
            var operand = GenerateExpression(prefixOperation.Operand);
            return $"{@operator}{operand}";
        }

        public string VisitInfixOperation(InfixOperation infixOperation)
        {
            var leftOperandCode = GenerateExpression(infixOperation.LeftOperand);
            var operatorCode = GenerateInfixOperator(infixOperation.Operator);
            var rightOperandCode = GenerateExpression(infixOperation.RightOperand);
            return $"{leftOperandCode}{operatorCode}{rightOperandCode}";
        }

        public string VisitBooleanLiteral(BooleanLiteral booleanLiteral) =>
            booleanLiteral.Value switch
            {
                true => "true",
                false => "false",
            };

        public static string GenerateExpressionCode(IExpression expression)
        {
            var self = new ExpressionCodeGenerator();
            return $"({expression.Accept(self)})";
        }

        private static string GeneratePrefixOperator(PrefixOperator @operator) =>
            @operator.Kind switch
            {
                PrefixOperatorKind.Not => "!",
                PrefixOperatorKind.Identity => "+",
                PrefixOperatorKind.Negate => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private static string GenerateInfixOperator(InfixOperator @operator) =>
            @operator.Kind switch
            {
                InfixOperatorKind.Add => "+",
                InfixOperatorKind.Divide => "/",
                InfixOperatorKind.Equal => "==",
                InfixOperatorKind.Greater => "==",
                InfixOperatorKind.GreaterOrEqual => ">=",
                InfixOperatorKind.Less => "<",
                InfixOperatorKind.LessOrEqual => "<=",
                InfixOperatorKind.Multiply => "*",
                InfixOperatorKind.NotEqual => "!=",
                InfixOperatorKind.Subtract => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private string GenerateExpression(IExpression expression) => $"({expression.Accept(this)})";
    }
}

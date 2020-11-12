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
            var @operator = GeneratePrefixOperator(prefixOperation.Operator.Node);
            var operand = GenerateExpression(prefixOperation.Operand);
            return $"{@operator}{operand}";
        }

        public string VisitInfixOperation(InfixOperation infixOperation)
        {
            var leftOperandCode = GenerateExpression(infixOperation.LeftOperand);
            var operatorCode = GenerateInfixOperator(infixOperation.Operator.Node);
            var rightOperandCode = GenerateExpression(infixOperation.RightOperand);
            return $"{leftOperandCode}{operatorCode}{rightOperandCode}";
        }

        public string VisitBooleanLiteral(BooleanLiteral booleanLiteral) =>
            booleanLiteral.Value switch
            {
                true => "true",
                false => "false",
            };

        public string VisitParenthesizedExpression(
            ParenthesizedExpression parenthesizedExpression
        ) => GenerateExpression(parenthesizedExpression.Expression);

        public static string GenerateExpressionCode(IExpression expression)
        {
            var self = new ExpressionCodeGenerator();
            return $"({expression.Accept(self)})";
        }

        private static string GeneratePrefixOperator(PrefixOperator @operator) =>
            @operator switch
            {
                PrefixOperator.Not => "!",
                PrefixOperator.Identity => "+",
                PrefixOperator.Negate => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private static string GenerateInfixOperator(InfixOperator @operator) =>
            @operator switch
            {
                InfixOperator.Add => "+",
                InfixOperator.Divide => "/",
                InfixOperator.Equal => "==",
                InfixOperator.Greater => "==",
                InfixOperator.GreaterOrEqual => ">=",
                InfixOperator.Less => "<",
                InfixOperator.LessOrEqual => "<=",
                InfixOperator.Multiply => "*",
                InfixOperator.NotEqual => "!=",
                InfixOperator.Subtract => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private string GenerateExpression(IExpression expression) => $"({expression.Accept(this)})";
    }
}

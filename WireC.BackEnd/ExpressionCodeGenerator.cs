using System;
using System.Globalization;
using System.Text;

using WireC.AST;
using WireC.AST.Expressions;

namespace WireC.BackEnd
{
    public class ExpressionCodeGenerator : IExpressionVisitor<string>
    {
        private ExpressionCodeGenerator()
        {
        }

        public string VisitIdentifier(IdentifierLiteral identifier) => identifier.Name;

        public string VisitIntegerLiteral(IntegerLiteral integer) => integer.Value + "ll";

        public string VisitFunctionCall(FunctionCall functionCall)
        {
            var callee = GenerateExpression(functionCall.Callee);

            var arguments = new StringBuilder();
            for (var i = 0; i < functionCall.Arguments.Count; i++)
            {
                arguments.Append(GenerateExpression(functionCall.Arguments[i]));
                if (i < functionCall.Arguments.Count - 1) arguments.Append(", ");
            }

            return $"{callee}({arguments})";
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

        public string VisitFloatLiteral(FloatLiteral floatLiteral) =>
            floatLiteral.Value.ToString("0.0##############", CultureInfo.InvariantCulture);

        public static string GenerateExpressionCode(IExpression expression)
        {
            var self = new ExpressionCodeGenerator();
            return $"({expression.Accept(self)})";
        }

        private static string GeneratePrefixOperator(PrefixOperator @operator) =>
            @operator switch
            {
                PrefixOperator.BinaryNot => "~",
                PrefixOperator.Not => "!",
                PrefixOperator.Identity => "+",
                PrefixOperator.Negate => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private static string GenerateInfixOperator(InfixOperator @operator) =>
            @operator switch
            {
                InfixOperator.Add => "+",
                InfixOperator.BinaryAnd => "&",
                InfixOperator.BinaryOr => "|",
                InfixOperator.BinaryXor => "^",
                InfixOperator.Divide => "/",
                InfixOperator.Equal => "==",
                InfixOperator.Greater => "==",
                InfixOperator.GreaterOrEqual => ">=",
                InfixOperator.Less => "<",
                InfixOperator.LessOrEqual => "<=",
                InfixOperator.LogicalAnd => "&&",
                InfixOperator.LogicalOr => "||",
                InfixOperator.Modulo => "%",
                InfixOperator.Multiply => "*",
                InfixOperator.NotEqual => "!=",
                InfixOperator.ShiftLeft => "<<",
                InfixOperator.ShiftRight => ">>",
                InfixOperator.Subtract => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private string GenerateExpression(IExpression expression) => $"({expression.Accept(this)})";
    }
}

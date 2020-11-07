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

        public static string GenerateExpressionCode(IExpression expression)
        {
            var self = new ExpressionCodeGenerator();
            return $"({expression.Accept(self)})";
        }

        public string VisitIdentifier(Identifier identifier) => identifier.Name;

        public string VisitIntegerLiteral(IntegerLiteral integer) => integer.Token.Lexeme;

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

        private static string GeneratePrefixOperator(PrefixOperator @operator) =>
            @operator.Kind switch
            {
                PrefixOperatorKind.Identity => "+",
                PrefixOperatorKind.Negate => "-",
                _ => throw new ArgumentException(nameof(@operator)),
            };

        private string GenerateExpression(IExpression expression) => $"({expression.Accept(this)})";
    }
}

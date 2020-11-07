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

        private string GenerateExpression(IExpression expression) => $"({expression.Accept(this)})";
    }
}

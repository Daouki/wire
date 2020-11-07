using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class ExpressionAnalyzer : IExpressionVisitor<bool>
    {
        private Context _context;
        private Scope _environment;

        private ExpressionAnalyzer(Context context, Scope environment)
        {
            _context = context;
            _environment = environment;
        }

        public static bool IsExpressionValid(
            Context context,
            Scope environment,
            IExpression expression)
        {
            var self = new ExpressionAnalyzer(context, environment);
            return expression.Accept(self);
        }

        public bool VisitIdentifier(Identifier identifier) => true;

        public bool VisitIntegerLiteral(IntegerLiteral integer) => true;

        public bool VisitFunctionCall(FunctionCall functionCall)
        {
            var calleeType = Typer.GetExpressionType(_context, _environment, functionCall.Callee);
            return calleeType != null;
        }
    }
}

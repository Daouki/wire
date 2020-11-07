using WireC.AST;
using WireC.AST.Expressions;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class ExpressionAnalyzer : IExpressionVisitor<bool>
    {
        private Context _context;

        private ExpressionAnalyzer(Context context)
        {
            _context = context;
        }

        public static bool IsExpressionValid(Context context, IExpression expression)
        {
            var self = new ExpressionAnalyzer(context);
            return expression.Accept(self);
        }

        public bool VisitIdentifier(Identifier identifier) => true;

        public bool VisitIntegerLiteral(IntegerLiteral integer) => true;
    }
}

using WireC.AST;
using WireC.AST.Expressions;
using WireC.AST.Types;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class Typer : IExpressionVisitor<IType>
    {
        private readonly Context _context;

        private Typer(Context context, Scope environment)
        {
            _context = context;
            _environment = environment;
        }

        private readonly Scope _environment;

        public static IType GetExpressionType(
            Context context,
            Scope environment,
            IExpression expression)
        {
            var self = new Typer(context, environment);
            return expression.Accept(self);
        }

        private IType GetExpressionType(IExpression expression) => expression.Accept(this);

        public IType VisitIdentifier(Identifier identifier)
        {
            var symbol = _environment.GetSymbol(identifier.Name);
            if (symbol != null) return symbol.Type;
            _context.Error(
                identifier.Span,
                $"symbol \"{identifier.Name}\" was not declared in the current scope"
            );
            return null;
        }

        public IType VisitIntegerLiteral(IntegerLiteral integer) => new IntegerType();

        public IType VisitFunctionCall(FunctionCall functionCall)
        {
            var calleeType = GetExpressionType(functionCall.Callee);
            if (calleeType is FunctionType calleeFunctionType) return calleeFunctionType.ReturnType;

            _context.Error(
                functionCall.Callee.Span,
                "cannot call a non-callable type \"{calleType}\""
            );
            return null;
        }

        public IType VisitPrefixOperation(PrefixOperation prefixOperation)
        {
            return GetExpressionType(prefixOperation.Operand)
                .GetPrefixOperationResultType(prefixOperation.Operator.Kind);
        }
    }
}

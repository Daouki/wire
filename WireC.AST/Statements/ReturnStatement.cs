using WireC.Common;

namespace WireC.AST.Statements
{
    public class ReturnStatement : IStatement
    {
        public ReturnStatement(SourceSpan span, IExpression expression)
        {
            Span = span;
            Expression = expression;
        }

        public IExpression Expression { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitReturnStatement(this);
    }
}
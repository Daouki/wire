using WireC.Common;

namespace WireC.AST.Statements
{
    public class ExpressionStatement : IStatement
    {
        public ExpressionStatement(int nodeId, SourceSpan span, IExpression expression)
        {
            Expression = expression;
            NodeId = nodeId;
            Span = span;
        }

        public IExpression Expression { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitExpressionStatement(this);
    }
}

using WireC.Common;

namespace WireC.AST.Statements
{
    public class ReturnStatement : IStatement
    {
        public ReturnStatement(int nodeId, SourceSpan span, IExpression expression)
        {
            NodeId = nodeId;
            Span = span;
            Expression = expression;
        }

        public IExpression Expression { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitReturnStatement(this);
    }
}
using WireC.Common;

namespace WireC.AST.Statements
{
    public class AssertStatement : IStatement
    {
        public AssertStatement(int nodeId, SourceSpan span, IExpression condition)
        {
            NodeId = nodeId;
            Span = span;
            Condition = condition;
        }

        public IExpression Condition { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitAssertStatement(this);
    }
}
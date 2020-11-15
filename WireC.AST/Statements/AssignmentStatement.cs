using WireC.Common;

namespace WireC.AST.Statements
{
    public class AssignmentStatement : IStatement
    {
        public AssignmentStatement(
            int nodeId,
            SourceSpan span,
            IExpression target,
            IExpression value)
        {
            NodeId = nodeId;
            Span = span;
            Target = target;
            Value = value;
        }

        public IExpression Target { get; }
        public IExpression Value { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitAssignmentStatement(this);
    }
}

using WireC.Common;

namespace WireC.AST.Statements
{
    public class ContinueStatement : IStatement
    {
        public ContinueStatement(int nodeId, SourceSpan span)
        {
            NodeId = nodeId;
            Span = span;
        }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitContinueStatement(this);
    }
}
using WireC.Common;

namespace WireC.AST.Statements
{
    public class WhileStatement : IStatement
    {
        public WhileStatement(int nodeId, SourceSpan span, IExpression condition, Block body)
        {
            NodeId = nodeId;
            Span = span;
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }
        public Block Body { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitWhileStatement(this);
    }
}
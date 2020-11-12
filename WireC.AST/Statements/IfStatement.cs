using WireC.Common;

namespace WireC.AST.Statements
{
    public class IfStatement : IStatement
    {
        public IfStatement(
            int nodeId,
            SourceSpan span,
            IExpression condition,
            Block thenBody,
            Block elseBody)
        {
            NodeId = nodeId;
            Span = span;
            Condition = condition;
            ThenBody = thenBody;
            ElseBody = elseBody;
        }

        public IExpression Condition { get; }
        public Block ThenBody { get; }
        public Block ElseBody { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitIfStatement(this);
    }
}
using System.Collections.Generic;
using System.Diagnostics;

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
            List<Elif> elifs,
            Block elseBody)
        {
            Debug.Assert(elifs != null);

            NodeId = nodeId;
            Span = span;
            Condition = condition;
            ThenBody = thenBody;
            Elifs = elifs;
            ElseBody = elseBody;
        }

        public IExpression Condition { get; }
        public Block ThenBody { get; }
        public List<Elif> Elifs { get; }
        public Block ElseBody { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitIfStatement(this);
    }

    public class Elif
    {
        public Elif(IExpression condition, Block body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }
        public Block Body { get; }
    }
}
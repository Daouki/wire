using WireC.Common;

namespace WireC.AST.Statements
{
    public class FunctionDefinition : IStatement
    {
        public FunctionDefinition(
            int nodeId,
            SourceSpan span,
            Token name,
            Block body,
            ITypeSignature returnTypeSignature)
        {
            NodeId = nodeId;
            Span = span;
            Name = name;
            ReturnTypeSignature = returnTypeSignature;
            Body = body;
        }

        public int NodeId { get; }
        public Token Name { get; }
        public ITypeSignature ReturnTypeSignature { get; }
        public Block Body { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitFunctionDefinition(this);
    }
}

using WireC.Common;

namespace WireC.AST.Statements
{
    public class FunctionDefinition : IStatement
    {
        public FunctionDefinition(SourceSpan span, Token name, Block body)
        {
            Span = span;
            Name = name;
            Body = body;
        }

        public Token Name { get; }
        public Block Body { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitFunctionDefinition(this);
    }
}
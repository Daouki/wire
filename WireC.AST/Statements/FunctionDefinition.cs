using WireC.Common;

namespace WireC.AST.Statements
{
    public class FunctionDefinition : IStatement
    {
        public FunctionDefinition(
            SourceSpan span,
            Token name,
            Block body,
            ITypeSignature returnTypeSignature)
        {
            Span = span;
            Name = name;
            ReturnTypeSignature = returnTypeSignature;
            Body = body;
        }

        public Token Name { get; }
        public ITypeSignature ReturnTypeSignature { get; }
        public IType ReturnType { get; set; }
        public Block Body { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitFunctionDefinition(this);
    }
}

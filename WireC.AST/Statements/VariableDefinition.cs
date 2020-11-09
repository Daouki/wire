using WireC.Common;

namespace WireC.AST.Statements
{
    public class VariableDefinition : IStatement
    {
        public VariableDefinition(int nodeId, SourceSpan span, Token identifier, ITypeSignature typeSignature, IExpression initializer)
        {
            NodeId = nodeId;
            Span = span;
            Identifier = identifier;
            TypeSignature = typeSignature;
            Initializer = initializer;
        }

        public int NodeId { get; }
        public SourceSpan Span { get; }
        public Token Identifier { get; }
        public ITypeSignature TypeSignature { get; }
        public IExpression Initializer { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitVariableDefinition(this);
    }
}

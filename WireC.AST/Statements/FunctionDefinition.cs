using System.Collections.Generic;

using WireC.Common;

namespace WireC.AST.Statements
{
    public class FunctionDefinition : IStatement
    {
        public FunctionDefinition(
            int nodeId,
            SourceSpan span,
            Token identifier,
            List<Spanned<FunctionParameter>> parameters,
            Block body,
            ITypeSignature returnTypeSignature)
        {
            NodeId = nodeId;
            Span = span;
            Identifier = identifier;
            Parameters = parameters;
            ReturnTypeSignature = returnTypeSignature;
            Body = body;
        }

        public Token Identifier { get; }
        public List<Spanned<FunctionParameter>> Parameters { get; }
        public ITypeSignature ReturnTypeSignature { get; }
        public Block Body { get; }

        public int NodeId { get; }
        public SourceSpan Span { get; }

        public void Accept(IStatementVisitor visitor) => visitor.VisitFunctionDefinition(this);
    }

    public class FunctionParameter
    {
        public FunctionParameter(int nodeId, Token identifier, ITypeSignature typeSignature)
        {
            NodeId = nodeId;
            Identifier = identifier;
            TypeSignature = typeSignature;
        }

        public int NodeId { get; }
        public Token Identifier { get; }
        public ITypeSignature TypeSignature { get; }
    }
}
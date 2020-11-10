using WireC.Common;

namespace WireC.AST
{
    public interface IStatement : INode
    {
        SourceSpan Span { get; }

        void Accept(IStatementVisitor visitor);
    }
}
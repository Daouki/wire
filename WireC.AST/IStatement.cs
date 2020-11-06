using WireC.Common;

namespace WireC.AST
{
    public interface IStatement
    {
        SourceSpan Span { get; }

        void Accept(IStatementVisitor visitor);
    }
}
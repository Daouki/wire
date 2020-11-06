using WireC.AST.Statements;

namespace WireC.AST
{
    public interface IStatementVisitor
    {
        void VisitFunctionDefinition(FunctionDefinition functionDefinition);
        void VisitReturnStatement(ReturnStatement returnStatement);
    }
}
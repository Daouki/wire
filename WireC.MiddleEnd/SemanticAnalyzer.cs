using System.Collections.Generic;

using WireC.AST;
using WireC.AST.Statements;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class SemanticAnalyzer : IStatementVisitor
    {
        private readonly Context _context;
        private readonly List<IStatement> _abstractSyntaxTree;

        private FunctionDefinition _functionContext;

        public SemanticAnalyzer(Context context, List<IStatement> abstractSyntaxTree)
        {
            _context = context;
            _abstractSyntaxTree = abstractSyntaxTree;
        }

        public void Analyze()
        {
            foreach (var statement in _abstractSyntaxTree) Analyze(statement);
        }

        private void Analyze(IStatement statement) => statement.Accept(this);

        private void AnalyzeBlock(Block block)
        {
            foreach (var statement in block.Statements) Analyze(statement);
        }

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            _functionContext = functionDefinition;

            AnalyzeBlock(functionDefinition.Body);

            _functionContext = null;
        }

        public void VisitReturnStatement(ReturnStatement returnStatement)
        {
            if (_functionContext == null)
                _context.Error(returnStatement.Span, "return statement outside of a function");

            ExpressionAnalyzer.IsExpressionValid(_context, returnStatement.Expression);
        }
    }
}

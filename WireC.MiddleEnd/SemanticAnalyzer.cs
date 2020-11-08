using System.Collections.Generic;

using WireC.AST;
using WireC.AST.Statements;
using WireC.AST.Types;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class SemanticAnalyzer : IStatementVisitor
    {
        private readonly Context _context;
        private readonly List<IStatement> _abstractSyntaxTree;
        private readonly ASTContext _astContext;

        private Scope _currentScope;
        private FunctionDefinition _functionContext;

        public SemanticAnalyzer(
            Context context,
            List<IStatement> abstractSyntaxTree,
            ASTContext astContext)
        {
            _context = context;
            _abstractSyntaxTree = abstractSyntaxTree;
            _astContext = astContext;
        }

        public void Analyze()
        {
            _currentScope = new Scope(); // Create the global scope.
            foreach (var statement in _abstractSyntaxTree) Analyze(statement);
        }

        private void Analyze(IStatement statement) => statement.Accept(this);

        private void AnalyzeBlock(Block block)
        {
            foreach (var statement in block.Statements) Analyze(statement);
        }

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            if (functionDefinition.Name.Lexeme == "main")
                _astContext.AddMangledName(functionDefinition.NodeId, "wiz_main__");

            var returnType = TypeSignatureParser.ParseTypeSignature(
                _context,
                functionDefinition.ReturnTypeSignature
            );
            if (returnType != null)
            {
                var functionType = new FunctionType(returnType);
                _astContext.AddNodeType(functionDefinition.NodeId, functionType);
                var wasRedefined = !_currentScope.DefineSymbol(functionDefinition, functionType);
                if (wasRedefined)
                {
                    _context.Error(
                        functionDefinition.Name.Span,
                        $"redefinition of function \"{functionDefinition.Name}\""
                    );
                }
            }

            _functionContext = functionDefinition;
            AnalyzeBlock(functionDefinition.Body);
            _functionContext = null;
        }

        public void VisitReturnStatement(ReturnStatement returnStatement)
        {
            if (_functionContext == null)
                _context.Error(returnStatement.Span, "return statement outside of a function");

            ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                returnStatement.Expression
            );
        }
    }
}

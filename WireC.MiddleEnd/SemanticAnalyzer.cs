using System.Collections.Generic;
using System.Diagnostics;

using WireC.AST;
using WireC.AST.Statements;
using WireC.AST.Types;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class SemanticAnalyzer : IStatementVisitor
    {
        private readonly List<IStatement> _abstractSyntaxTree;
        private readonly ASTContext _astContext;
        private readonly Context _context;

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

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            if (functionDefinition.Name.Lexeme == "main")
                _astContext.AddMangledName(functionDefinition.NodeId, "wiz_main__");

            var returnType = functionDefinition.ReturnTypeSignature != null
                ? TypeSignatureParser.ParseTypeSignature(
                    _context,
                    functionDefinition.ReturnTypeSignature
                )
                : new VoidType();
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

        public void VisitVariableDefinition(VariableDefinition variableDefinition)
        {
            if (variableDefinition.TypeSignature != null && variableDefinition.Initializer != null)
            {
                var variableType = TypeSignatureParser.ParseTypeSignature(
                    _context,
                    variableDefinition.TypeSignature
                );

                var initializerType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer
                );

                if (variableType != null && !variableType.IsSame(initializerType))
                {
                    // We made sure in the StatementParser that a definition without both initializer
                    // and type signature is not a valid AST item and discarded it with an error.
                    Debug.Assert(variableDefinition.Initializer != null);
                    _context.Error(
                        variableDefinition.Initializer.Span,
                        $"expected type \"{variableType}\", but found \"{initializerType}\""
                    );
                }

                _astContext.AddNodeType(variableDefinition.NodeId, variableType);
            }
            else if (variableDefinition.TypeSignature != null &&
                variableDefinition.Initializer == null)
            {
                var variableType =
                    TypeSignatureParser.ParseTypeSignature(
                        _context,
                        variableDefinition.TypeSignature
                    );

                _astContext.AddNodeType(variableDefinition.NodeId, variableType);
            }
            else // variableDefinition.TypeSignature == null && variableDefinition.Initializer != null
            {
                var initializerType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer
                );
                if (initializerType is VoidType)
                {
                    // Again, the linter is overly sensitive. There's no way for the Initializer
                    // field to be null at this point.
                    Debug.Assert(variableDefinition.Initializer != null);
                    _context.Error(
                        variableDefinition.Initializer.Span,
                        "type \"void\" cannot be assigned to a variable"
                    );
                }

                _astContext.AddNodeType(variableDefinition.NodeId, initializerType);
            }

            if (!_currentScope.DefineSymbol(
                variableDefinition,
                _astContext.GetNodeType(variableDefinition.NodeId)
            ))
            {
                _context.Error(
                    variableDefinition.Span,
                    $"redefined previously defined symbol \"{variableDefinition.Identifier}\""
                );
            }
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
    }
}

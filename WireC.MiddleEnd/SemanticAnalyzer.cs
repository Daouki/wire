﻿using System.Collections.Generic;
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
            if (functionDefinition.Identifier.Lexeme == "main")
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
                        functionDefinition.Identifier.Span,
                        $"redefinition of function \"{functionDefinition.Identifier}\""
                    );
                }
            }

            _functionContext = functionDefinition;
            _currentScope = new Scope(_currentScope);

            foreach (var parameter in functionDefinition.Parameters)
            {
                var parameterType =
                    TypeSignatureParser.ParseTypeSignature(_context, parameter.Node.TypeSignature);
                if (parameterType != null)
                    _astContext.AddNodeType(parameter.Node.NodeId, parameterType);
                _currentScope.DefineSymbol(parameter.Node, parameterType);
            }

            AnalyzeBlock(functionDefinition.Body);

            _currentScope = _currentScope.Outer;
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

                if (!ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer
                ))
                    return;

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
                if (!ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer
                ))
                    return;

                var initializerType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer
                );
                switch (initializerType)
                {
                    case VoidType _:
                        // Again, the linter is overly sensitive. There's no way for the Initializer
                        // field to be null at this point.
                        Debug.Assert(variableDefinition.Initializer != null);
                        _context.Error(
                            variableDefinition.Initializer.Span,
                            "type \"void\" cannot be assigned to a variable"
                        );
                        break;
                    case null:
                        return;
                    default:
                        _astContext.AddNodeType(variableDefinition.NodeId, initializerType);
                        break;
                }
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

        public void VisitAssertStatement(AssertStatement assertStatement)
        {
            if (!ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                assertStatement.Condition
            ))
                return;

            var conditionType = Typer.GetExpressionType(
                _context,
                _currentScope,
                assertStatement.Condition
            );
            if (conditionType != null && conditionType is not BooleanType)
            {
                _context.Error(
                    assertStatement.Condition.Span,
                    $"type mismatch; expected \"bool\", but found \"{conditionType}\""
                );
            }
        }

        public void VisitIfStatement(IfStatement ifStatement)
        {
            if (!ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                ifStatement.Condition
            ))
                return;

            var conditionType = Typer.GetExpressionType(
                _context,
                _currentScope,
                ifStatement.Condition
            );
            if (conditionType != null && conditionType is not BooleanType)
            {
                _context.Error(
                    ifStatement.Condition.Span,
                    "condition does not evaluate to \"bool\" type"
                );
            }

            AnalyzeBlock(ifStatement.ThenBody);
            if (ifStatement.ElseBody != null) AnalyzeBlock(ifStatement.ElseBody);
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
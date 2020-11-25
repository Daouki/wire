using System.Collections.Generic;
using System.Diagnostics;

using WireC.AST;
using WireC.AST.Expressions;
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

        private int _loopNestLevel;

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

            var (parameterTypes, allParamsOk) = GetFunctionParameterTypes(functionDefinition);
            if (!allParamsOk) return;

            var returnType = GetFunctionReturnType(functionDefinition);
            if (returnType != null)
            {
                var functionType = new FunctionType(parameterTypes, returnType);
                _astContext.AddNodeType(functionDefinition.NodeId, functionType);
                var wasRedefined = !_currentScope.DefineSymbol(functionDefinition, functionType);
                if (wasRedefined)
                {
                    _context.Error(
                        functionDefinition.Identifier.Span,
                        $"redefinition of function \"{functionDefinition.Identifier}\"");
                }
            }

            _functionContext = functionDefinition;
            _currentScope = new Scope(_currentScope);

            foreach (var parameter in functionDefinition.Parameters)
            {
                var parameterType = _astContext.GetNodeType(parameter.Node.NodeId);
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
                returnStatement.Expression);
        }

        public void VisitVariableDefinition(VariableDefinition variableDefinition)
        {
            if (variableDefinition.TypeSignature != null && variableDefinition.Initializer != null)
            {
                var variableType = TypeSignatureParser.ParseTypeSignature(
                    _context,
                    variableDefinition.TypeSignature);

                if (!ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer))
                    return;

                var initializerType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer);

                if (variableType != null && !variableType.IsSame(initializerType))
                {
                    // We made sure in the StatementParser that a definition without both initializer
                    // and type signature is not a valid AST item and discarded it with an error.
                    Debug.Assert(variableDefinition.Initializer != null);
                    _context.Error(
                        variableDefinition.Initializer.Span,
                        $"expected type \"{variableType}\", but found \"{initializerType}\"");
                }

                _astContext.AddNodeType(variableDefinition.NodeId, variableType);
            }
            else if (variableDefinition.TypeSignature != null &&
                variableDefinition.Initializer == null)
            {
                var variableType =
                    TypeSignatureParser.ParseTypeSignature(
                        _context,
                        variableDefinition.TypeSignature);

                _astContext.AddNodeType(variableDefinition.NodeId, variableType);
            }
            else // variableDefinition.TypeSignature == null && variableDefinition.Initializer != null
            {
                if (!ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer))
                    return;

                var initializerType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    variableDefinition.Initializer);
                switch (initializerType)
                {
                    case VoidType _:
                        // Again, the linter is overly sensitive. There's no way for the Initializer
                        // field to be null at this point.
                        Debug.Assert(variableDefinition.Initializer != null);
                        _context.Error(
                            variableDefinition.Initializer.Span,
                            "type \"void\" cannot be assigned to a variable");
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
                _astContext.GetNodeType(variableDefinition.NodeId)))
            {
                _context.Error(
                    variableDefinition.Span,
                    $"redefined previously defined symbol \"{variableDefinition.Identifier}\"");
            }
        }

        public void VisitAssertStatement(AssertStatement assertStatement)
        {
            if (!ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                assertStatement.Condition))
                return;

            var conditionType = Typer.GetExpressionType(
                _context,
                _currentScope,
                assertStatement.Condition);
            if (conditionType != null && !(conditionType is BooleanType))
            {
                _context.Error(
                    assertStatement.Condition.Span,
                    $"type mismatch; expected \"bool\", but found \"{conditionType}\"");
            }
        }

        public void VisitIfStatement(IfStatement ifStatement)
        {
            if (!ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                ifStatement.Condition))
                return;

            var conditionType = Typer.GetExpressionType(
                _context,
                _currentScope,
                ifStatement.Condition);
            if (conditionType != null && !(conditionType is BooleanType))
            {
                _context.Error(
                    ifStatement.Condition.Span,
                    "condition does not evaluate to \"bool\" type");
            }

            AnalyzeBlock(ifStatement.ThenBody);
            if (ifStatement.ElseBody != null) AnalyzeBlock(ifStatement.ElseBody);
        }

        public void VisitExpressionStatement(ExpressionStatement expressionStatement) =>
            ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                expressionStatement.Expression);

        public void VisitWhileStatement(WhileStatement whileStatement)
        {
            if (ExpressionAnalyzer.IsExpressionValid(
                _context,
                _currentScope,
                whileStatement.Condition))
            {
                var conditionType = Typer.GetExpressionType(
                    _context,
                    _currentScope,
                    whileStatement.Condition);
                if (conditionType != null && !conditionType.IsSame(new BooleanType()))
                {
                    _context.Error(
                        whileStatement.Condition.Span,
                        "while loop condition does not evaluate to type \"bool\"");
                }
            }

            _loopNestLevel++;
            AnalyzeBlock(whileStatement.Body);
            _loopNestLevel--;
        }

        public void VisitAssignmentStatement(AssignmentStatement assignmentStatement)
        {
            if (!ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    assignmentStatement.Target) ||
                !ExpressionAnalyzer.IsExpressionValid(
                    _context,
                    _currentScope,
                    assignmentStatement.Value))
                return;

            if (!IsLValue(assignmentStatement.Target))
            {
                _context.Error(
                    assignmentStatement.Target.Span,
                    "left-hand of the assignment is not assignable");
                return;
            }

            var targetType = Typer.GetExpressionType(
                _context,
                _currentScope,
                assignmentStatement.Target);
            var valueType = Typer.GetExpressionType(
                _context,
                _currentScope,
                assignmentStatement.Value);
            if (targetType == null || valueType == null) return;
            if (!targetType.IsSame(valueType))
            {
                _context.Error(
                    assignmentStatement.Value.Span,
                    $"type mismatch; expected \"{targetType}\", but found \"{valueType}\"");
            }
        }

        public void VisitBreakStatement(BreakStatement breakStatement)
        {
            if (_loopNestLevel == 0)
                _context.Error(breakStatement.Span, @"""break"" statement used outside of a loop");
        }

        public void VisitContinueStatement(ContinueStatement continueStatement)
        {
            if (_loopNestLevel == 0)
            {
                _context.Error(
                    continueStatement.Span,
                    @"""continue"" statement used outside of a loop");
            }
        }

        private static bool IsLValue(IExpression expression) => expression switch
        {
            IdentifierLiteral _ => true,
            SubscriptExpression _ => true,
            _ => false,
        };

        private IType GetFunctionReturnType(FunctionDefinition function) =>
            function.ReturnTypeSignature != null
                ? TypeSignatureParser.ParseTypeSignature(
                    _context,
                    function.ReturnTypeSignature)
                : new VoidType();

        private (List<IType>, bool) GetFunctionParameterTypes(FunctionDefinition function)
        {
            var parameterTypes = new List<IType>();
            var success = true;
            foreach (var parameter in function.Parameters)
            {
                var parameterType = TypeSignatureParser.ParseTypeSignature(
                    _context,
                    parameter.Node.TypeSignature);
                if (parameterType == null) success = false;
                _astContext.AddNodeType(parameter.Node.NodeId, parameterType);
                parameterTypes.Add(parameterType);
            }

            return (parameterTypes, success);
        }

        public void Analyze()
        {
            _currentScope = new Scope(); // Create the global scope.
            foreach (var statement in _abstractSyntaxTree) Analyze(statement);
        }

        private void Analyze(IStatement statement) => statement.Accept(this);

        private void AnalyzeBlock(Block block)
        {
            _currentScope = new Scope(_currentScope);
            foreach (var statement in block.Statements) Analyze(statement);
            _currentScope = _currentScope.Outer;
        }
    }
}
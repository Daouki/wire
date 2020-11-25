using System.Collections.Generic;
using System.Text;

using WireC.AST;
using WireC.AST.Statements;
using WireC.AST.Types;
using WireC.Common;

namespace WireC.BackEnd
{
    public class CodeGenerator : IStatementVisitor
    {
        /// <summary>
        /// Prologue is always included at the beginning of the generated program.
        /// </summary>
        private const string _prologue = @"#include <cstdio>
#include <cstdlib>
#include <cstdint>

#include <array>

#define WIRE_ASSERT__(File, Line, Column, CCond, WCond) do { if (!(CCond)) { \
        std::fprintf(stderr, ""%s:%d:%d\nassertion failed: %s\n"", File, Line, Column, WCond); \
        std::abort(); \
    }} while (false)

";

        /// <summary>
        /// Epilogue is always included at the end of the generated program.
        /// </summary>
        private const string _epilogue = @"
int main() {
return (int)wiz_main__();
}
";

        /// <summary>
        /// The abstract syntax tree to be generated.
        /// </summary>
        private readonly List<IStatement> _abstractSyntaxTree;

        private readonly ASTContext _astContext;

        private readonly Context _context;

        /// <summary>
        /// The destination code generated so far.
        /// </summary>
        private readonly StringBuilder _generatedCode = new StringBuilder();

        public CodeGenerator(
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
            var nodeId = functionDefinition.NodeId;

            _generatedCode
                .Append(
                    TypeSignatureGenerator.GenerateTypeSignature(
                        ((FunctionType) _astContext.GetNodeType(nodeId)).ReturnType))
                .Append(' ')
                .Append(
                    _astContext.GetNodeMangledName(nodeId) ?? functionDefinition.Identifier.Lexeme)
                .Append('(');

            for (var i = 0; i < functionDefinition.Parameters.Count; i++)
            {
                var parameter = functionDefinition.Parameters[i];
                _generatedCode
                    .Append(
                        TypeSignatureGenerator.GenerateTypeSignature(
                            _astContext.GetNodeType(parameter.Node.NodeId)))
                    .Append(' ')
                    .Append(parameter.Node.Identifier);
                if (i < functionDefinition.Parameters.Count - 1) _generatedCode.Append(", ");
            }

            _generatedCode.Append(") ");
            GenerateBlockCode(functionDefinition.Body);
        }

        public void VisitReturnStatement(ReturnStatement returnStatement)
        {
            _generatedCode
                .Append("return ")
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(returnStatement.Expression))
                .Append(";\n");
        }

        public void VisitVariableDefinition(VariableDefinition variableDefinition)
        {
            _generatedCode
                .Append(
                    TypeSignatureGenerator.GenerateTypeSignature(
                        _astContext.GetNodeType(variableDefinition.NodeId)))
                .Append(' ')
                .Append(variableDefinition.Identifier);

            if (variableDefinition.Initializer != null)
            {
                _generatedCode
                    .Append(" = ")
                    .Append(
                        ExpressionCodeGenerator.GenerateExpressionCode(
                            variableDefinition.Initializer));
            }

            _generatedCode.Append(";\n");
        }

        public void VisitAssertStatement(AssertStatement assertStatement)
        {
            _generatedCode
                .Append("WIRE_ASSERT__(\"")
                .Append(_context.SourceFile.FilePath)
                .Append("\", ")
                .Append(assertStatement.Span.Line)
                .Append(", ")
                .Append(assertStatement.Span.Column)
                .Append(", ")
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(assertStatement.Condition))
                .Append(", \"")
                .Append(_context.SourceFile.ReadSpan(assertStatement.Condition.Span))
                .Append("\");\n");
        }

        public void VisitIfStatement(IfStatement ifStatement)
        {
            _generatedCode
                .Append("if (")
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(ifStatement.Condition))
                .Append(") ");
            GenerateBlockCode(ifStatement.ThenBody);

            if (ifStatement.ElseBody == null) return;
            _generatedCode.Append("else ");
            GenerateBlockCode(ifStatement.ElseBody);
        }

        public void VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            _generatedCode
                .Append(
                    ExpressionCodeGenerator.GenerateExpressionCode(expressionStatement.Expression))
                .Append(";\n");
        }

        public void VisitWhileStatement(WhileStatement whileStatement)
        {
            _generatedCode
                .Append("while (")
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(whileStatement.Condition))
                .Append(") ");
            GenerateBlockCode(whileStatement.Body);
        }

        public void VisitAssignmentStatement(AssignmentStatement assignmentStatement)
        {
            _generatedCode
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(assignmentStatement.Target))
                .Append(" = ")
                .Append(ExpressionCodeGenerator.GenerateExpressionCode(assignmentStatement.Value))
                .Append(";\n");
        }

        public void VisitBreakStatement(BreakStatement breakStatement)
        {
            _generatedCode.Append("break;\n");
        }

        public void VisitContinueStatement(ContinueStatement continueStatement)
        {
            _generatedCode.Append("continue;\n");
        }

        public string GenerateCode()
        {
            foreach (var statement in _abstractSyntaxTree) GenerateStatementCode(statement);
            return _prologue + _generatedCode + _epilogue;
        }

        private void GenerateStatementCode(IStatement statement) => statement.Accept(this);

        private void GenerateBlockCode(Block block)
        {
            _generatedCode.Append("{\n");
            foreach (var statement in block.Statements) GenerateStatementCode(statement);
            _generatedCode.Append("}\n");
        }
    }
}
using System.Collections.Generic;
using System.Text;

using WireC.AST;
using WireC.AST.Statements;
using WireC.AST.Types;

namespace WireC.BackEnd
{
    public class CodeGenerator : IStatementVisitor
    {
        /// <summary>
        /// Prologue is always included at the beginning of the generated program.
        /// </summary>
        private const string _prologue = @"#include<cstdint>

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

        /// <summary>
        /// The destination code generated so far.
        /// </summary>
        private readonly StringBuilder _generatedCode = new StringBuilder();

        public CodeGenerator(List<IStatement> abstractSyntaxTree, ASTContext astContext)
        {
            _abstractSyntaxTree = abstractSyntaxTree;
            _astContext = astContext;
        }

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            var nodeId = functionDefinition.NodeId;

            _generatedCode
                .Append(
                    TypeSignatureGenerator.GenerateTypeSignature(
                        ((FunctionType) _astContext.GetNodeType(nodeId)).ReturnType
                    )
                )
                .Append(' ')
                .Append(_astContext.GetNodeMangledName(nodeId) ?? functionDefinition.Name.Lexeme)
                .Append("() ");
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
                        _astContext.GetNodeType(variableDefinition.NodeId)
                    )
                )
                .Append(' ')
                .Append(variableDefinition.Identifier);

            if (variableDefinition.Initializer != null)
            {
                _generatedCode
                    .Append(" = ")
                    .Append(
                        ExpressionCodeGenerator.GenerateExpressionCode(
                            variableDefinition.Initializer
                        )
                    );
            }

            _generatedCode.Append(";\n");
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
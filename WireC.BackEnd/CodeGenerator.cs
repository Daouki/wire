using System.Collections.Generic;
using System.Text;

using WireC.AST;
using WireC.AST.Statements;

namespace WireC.BackEnd
{
    public class CodeGenerator : IStatementVisitor
    {
        /// <summary>
        /// The abstract syntax tree to be generated.
        /// </summary>
        private List<IStatement> _abstractSyntaxTree;

        /// <summary>
        /// Prelude is always included at the beginning of the generated program.
        /// </summary>
        private const string _prelude = @"#include<cstdint>

";

        /// <summary>
        /// The destination code generated so far.
        /// </summary>
        private StringBuilder _generatedCode = new StringBuilder();

        public CodeGenerator(List<IStatement> abstractSyntaxTree)
        {
            _abstractSyntaxTree = abstractSyntaxTree;
        }

        public string GenerateCode()
        {
            foreach (var statement in _abstractSyntaxTree) GenerateStatementCode(statement);
            return _prelude + _generatedCode;
        }

        private void GenerateStatementCode(IStatement statement) => statement.Accept(this);

        private void GenerateBlockCode(Block block)
        {
            _generatedCode.Append("{\n");
            foreach (var statement in block.Statements) GenerateStatementCode(statement);
            _generatedCode.Append("}\n");
        }

        public void VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            _generatedCode
                .Append(functionDefinition.Name.Lexeme == "main" ? "int " : "int64_t ")
                .Append(functionDefinition.Name)
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
    }
}

using WireC.AST.Statements;
using WireC.Common;

using Xunit;

namespace WireC.FrontEnd.Tests
{
    public class StatementParserTests
    {
        [Fact]
        public void ParsesVariableDefinition_WithTypeSignatureButWithoutInitializer()
        {
            const string input = "var x: i64;";
            var ast = Parser.Parse(new Context(new Options()), new Lexer(input).Tokenize());

            Assert.Single(ast);

            var astNode = ast[0];
            Assert.IsType<VariableDefinition>(astNode);

            var varDefinition = (VariableDefinition) astNode;
            Assert.NotNull(varDefinition.TypeSignature);
            Assert.Null(varDefinition.Initializer);
        }

        [Fact]
        public void ParsesVariableDefinition_WithInitializerButWithoutTypeSignature()
        {
            const string input = "var x = 42;";
            var ast = Parser.Parse(new Context(new Options()), new Lexer(input).Tokenize());

            Assert.Single(ast);

            var astNode = ast[0];
            Assert.IsType<VariableDefinition>(astNode);

            var varDefinition = (VariableDefinition) astNode;
            Assert.Null(varDefinition.TypeSignature);
            Assert.NotNull(varDefinition.Initializer);
        }

        [Fact]
        public void ParsesVariableDefinition_WithTypeSignatureAndInitializer()
        {
            const string input = "var x: i64 = 42;";
            var ast = Parser.Parse(new Context(new Options()), new Lexer(input).Tokenize());

            Assert.Single(ast);

            var astNode = ast[0];
            Assert.IsType<VariableDefinition>(astNode);

            var varDefinition = (VariableDefinition) astNode;
            Assert.NotNull(varDefinition.TypeSignature);
            Assert.NotNull(varDefinition.Initializer);
        }
    }
}
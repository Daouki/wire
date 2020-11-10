using WireC.Common;

using Xunit;

namespace WireC.FrontEnd.Tests
{
    public class LexerTests
    {
        [Theory]
        [InlineData("asdf", true)]
        [InlineData("ASDF", true)]
        [InlineData("aSdF", true)]
        [InlineData("1234asdf", false)]
        [InlineData(" \t\nasdf\r", true)]
        [InlineData("こんにちは", true)]
        [InlineData("こんにchiは", true)]
        public void TokenizesIdentifier(string input, bool shouldSucceed)
        {
            var tokens = new Lexer(input).Tokenize();
            var result = tokens[0];

            if (shouldSucceed)
            {
                Assert.Equal(TokenKind.Identifier, result.Kind);
                Assert.Equal(input.Trim(), result.Lexeme);
            }
            else
                Assert.NotEqual(TokenKind.Identifier, result.Kind);
        }

        [Theory]
        [InlineData(":", TokenKind.Colon)]
        public void TokenizesOperators(string input, TokenKind expected)
        {
            var tokens = new Lexer(input).Tokenize();
            var result = tokens[0];

            Assert.Equal(expected, result.Kind);
        }

        [Fact]
        public void AppendsEndOfFile()
        {
            const string input = "";
            var result = new Lexer(input).Tokenize();

            Assert.Single(result);
            Assert.Equal(TokenKind.EndOfFile, result[0].Kind);
        }

        [Fact]
        public void IgnoresWhitespace()
        {
            const string input = " \t\n\r\f\v";
            var result = new Lexer(input).Tokenize();

            Assert.Single(result);
            Assert.Equal(TokenKind.EndOfFile, result[0].Kind);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("+1234")]
        [InlineData("-1234")]
        public void TokenizesInteger(string input)
        {
            var tokens = new Lexer(input).Tokenize();
            var result = tokens[0];

            Assert.Equal(TokenKind.Integer, result.Kind);
            Assert.Equal(input.Trim(), result.Lexeme);
        }

        [Fact]
        public void TokenizesInvalid()
        {
            const string input = "@";
            var tokens = new Lexer(input).Tokenize();
            var result = tokens[0];

            Assert.Equal(TokenKind.Invalid, result.Kind);
            Assert.Equal(input.Trim(), result.Lexeme);
        }

        [Fact]
        public void IgnoresComment()
        {
            const string input = "-- Nothing to see here :^)";
            var tokens = new Lexer(input).Tokenize();
            var result = tokens[0];

            Assert.Equal(TokenKind.EndOfFile, result.Kind);
        }
    }
}
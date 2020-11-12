using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using WireC.Common;

namespace WireC.FrontEnd
{
    public class ParserState
    {
        /// <summary>
        /// The collection of tokens to be parsed into abstract syntax tree.
        /// </summary>
        private readonly List<Token> _tokens;

        /// <summary>
        /// Index of the token under examination.
        /// </summary>
        private int _position;

        public ParserState(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public NodeIdGenerator NodeIdGenerator { get; } = new NodeIdGenerator();

        /// <summary>
        /// Checks if there are any tokens left to be parsed.
        /// </summary>
        /// <returns></returns>
        public bool IsAtEnd() => _tokens[_position].Kind == TokenKind.EndOfFile;

        public Token Current() => _tokens[_position];

        public Token Previous() => _tokens[_position - 1];

        public void Advance()
        {
            if (!IsAtEnd()) _position++;
        }

        /// <summary>
        /// Takes a left-most unparsed token and if it matches the given kind, advances to the next token.
        /// </summary>
        /// <remarks>Trying to consume TokenKind.EndOfFile sets the parser in an undefined state.</remarks>
        /// <param name="kind">Token type that may be consumed.</param>
        /// <returns>True if consumed a token; false otherwise.</returns>
        public bool Consume(TokenKind kind)
        {
            if (_tokens[_position].Kind != kind) return false;
            _position++;
            return true;
        }

        /// <summary>
        /// Takes a left-most unparsed token and if it matches any of the given kinds, advances
        /// to the next token.
        /// </summary>
        /// <remarks>Trying to consume TokenKind.EndOfFile sets the parser in an undefined state.</remarks>
        /// <param name="kinds">List of kinds that may be consumed.</param>
        /// <returns>True if consumed a token; false otherwise.</returns>
        public bool Consume(params TokenKind[] kinds)
        {
            if (kinds.All(kind => _tokens[_position].Kind != kind)) return false;
            _position++;
            return true;
        }

        public Token ConsumeOrError(TokenKind kind, string expectedLexeme = null)
        {
            if (Consume(kind)) return Previous();
            var current = Current();
            throw new ParseException(
                current.Span,
                $"expected {expectedLexeme ?? GetTokenKindName(kind)}, but found \"{current}\""
            );
        }

        private static string GetTokenKindName(TokenKind kind)
        {
            var type = kind.GetType();
            var memberInfo = type.GetMember(kind.ToString());
            if (memberInfo.Length <= 0) return kind.ToString();
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0
                ? ((DescriptionAttribute) attrs[0]).Description
                : kind.ToString();
        }
    }
}

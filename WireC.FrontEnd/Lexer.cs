using System.Collections.Generic;
using System.Text.RegularExpressions;

using WireC.Common;

namespace WireC.FrontEnd
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenKind> _keywords =
            new Dictionary<string, TokenKind>
            {
                {"false", TokenKind.False},
                {"fn", TokenKind.Fn},
                {"return", TokenKind.Return},
                {"true", TokenKind.True},
                {"var", TokenKind.Var},
            };

        private static readonly Dictionary<string, TokenKind> _shortOperators =
            new Dictionary<string, TokenKind>
            {
                {"*", TokenKind.Asterisk},
                {"!", TokenKind.Bang},
                {":", TokenKind.Colon},
                {"=", TokenKind.Equal},
                {">", TokenKind.Greater},
                {"{", TokenKind.LeftBrace},
                {"(", TokenKind.LeftParenthesis},
                {"-", TokenKind.Minus},
                {"+", TokenKind.Plus},
                {"}", TokenKind.RightBrace},
                {")", TokenKind.RightParenthesis},
                {"<", TokenKind.Less},
                {";", TokenKind.Semicolon},
                {"/", TokenKind.Slash},
            };

        private static readonly Dictionary<string, TokenKind> _longOperators =
            new Dictionary<string, TokenKind>
            {
                {"==", TokenKind.EqualEqual},
                {">=", TokenKind.GreaterEqual},
                {"<=", TokenKind.LessEqual},
                {"<>", TokenKind.LessGreater},
            };

        private static readonly Regex _commentRegex = new Regex(
            @"(?:--[^\n]*)",
            RegexOptions.Compiled
        );

        private static readonly Regex _integerRegex = new Regex(
            @"\G[-+]?[0-9]+",
            RegexOptions.Compiled
        );

        /// <summary>
        /// The source code to be tokenized.
        /// </summary>
        private readonly string _sourceCode;

        private readonly List<Token> _tokens = new List<Token>();

        /// <summary>
        /// Keeps track of the number of the line that's being currently examined.
        /// </summary>
        private int _currentLine = 1;

        /// <summary>
        /// Index of a character in the input string that's currently being examined.
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// Keeps the index of the lastly seen new line '\n' character. Used to calculate the column
        /// position of a token.
        /// </summary>
        private int _lastNewLine;

        /// <summary>
        /// Index of a character in the input string where the reading of a next token has started.
        /// </summary>
        private int _startPosition;

        public Lexer(string sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public List<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                _startPosition = _currentPosition;

                var c = Advance();
                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n')
                    {
                        _currentLine++;
                        _lastNewLine = _currentPosition;
                    }

                    continue;
                }

                if (TryMatchRegex(_commentRegex)) continue;

                if (IsIdentifierStart(c))
                {
                    TokenizeIdentifierOrKeyword();
                    continue;
                }

                if (TryTokenizeInteger()) continue;

                if (TryAddLongOperator()) continue;

                if (TryParseShortOperator()) continue;

                AddToken(TokenKind.Invalid);
            }

            AddEndOfFileToken();
            return _tokens;
        }

        private bool TryParseShortOperator()
        {
            var maybeShortOperator = _sourceCode.Substring(_startPosition, 1);
            if (!_shortOperators.TryGetValue(maybeShortOperator, out var shortOp)) return false;
            AddToken(shortOp);
            return true;
        }

        private bool TryAddLongOperator()
        {
            if (_sourceCode.Length - _startPosition >= 2)
            {
                var maybeLongOperator = _sourceCode.Substring(_startPosition, 2);
                if (_longOperators.TryGetValue(maybeLongOperator, out var longOp))
                {
                    Advance();
                    AddToken(longOp);
                    return true;
                }
            }

            return false;
        }

        private void TokenizeIdentifierOrKeyword()
        {
            while (!IsAtEnd() && IsIdentifierContinuation(CurrentChar())) Advance();
            AddToken(
                _keywords.TryGetValue(
                    _sourceCode[_startPosition.._currentPosition],
                    out var keyword
                )
                    ? keyword
                    : TokenKind.Identifier
            );
        }

        private bool TryMatchRegex(Regex regex)
        {
            var match = regex.Match(_sourceCode, _startPosition);
            if (!match.Success || match.Index != _startPosition) return false;
            _currentPosition = _startPosition + match.Length;
            return true;
        }

        private bool TryTokenizeInteger()
        {
            if (!TryMatchRegex(_integerRegex)) return false;
            AddToken(TokenKind.Integer);
            return true;
        }

        /// <summary>
        /// Checks if there are any tokens left to be tokenized.
        /// </summary>
        private bool IsAtEnd() => _currentPosition >= _sourceCode.Length;

        /// <summary>
        /// Gets the char that's currently under examination.
        /// </summary>
        /// <returns>Char pointed by the _currentPosition index in the input string.</returns>
        private char CurrentChar() => _sourceCode[_currentPosition];

        /// <summary>
        /// Advances the current position pointer to the next character if isn't at the end of the input.
        /// </summary>
        /// <returns></returns>
        private char Advance()
        {
            if (!IsAtEnd()) _currentPosition++;
            return _sourceCode[_currentPosition - 1];
        }

        private void AddToken(TokenKind kind)
        {
            _tokens.Add(
                new Token
                {
                    Kind = kind,
                    Lexeme = _sourceCode[_startPosition.._currentPosition],
                    Span = new SourceSpan
                    {
                        Start = _startPosition,
                        End = _currentPosition,
                        Line = _currentLine,
                        Column = _startPosition - _lastNewLine + 1,
                    },
                }
            );
        }

        private void AddEndOfFileToken()
        {
            _tokens.Add(
                new Token
                {
                    Kind = TokenKind.EndOfFile,
                    Lexeme = "END_OF_FILE",
                    Span = new SourceSpan
                    {
                        Start = _currentPosition - 1,
                        End = _currentPosition,
                        Line = _currentLine,
                        Column = _startPosition - _lastNewLine + 1,
                    },
                }
            );
        }

        private static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';

        private static bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';

        private static bool IsIdentifierContinuation(char c) =>
            char.IsLetter(c) || IsAsciiDigit(c) || c == '_';
    }
}

using System.Collections.Generic;

using WireC.AST;
using WireC.Common;

namespace WireC.FrontEnd
{
    public static class Parser
    {
        public static List<IStatement> Parse(Context context, List<Token> tokens)
        {
            var state = new ParserState(tokens);
            return StatementParser.ParseStatementsUntil(context, state, TokenKind.EndOfFile);
        }
    }
}
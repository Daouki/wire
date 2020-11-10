using WireC.AST;
using WireC.Common;

namespace WireC.MiddleEnd
{
    public class Symbol
    {
        public Token Name { get; set; }
        public IType Type { get; set; }
    }
}
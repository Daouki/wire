using System.Collections.Generic;

namespace WireC.AST
{
    public class ASTContext
    {
        private Dictionary<int, string> _mangledNames = new Dictionary<int, string>();
        private Dictionary<int, IType> _types = new Dictionary<int, IType>();

        public void AddNodeType(int nodeId, IType type) => _types.Add(nodeId, type);

        public IType GetNodeType(int nodeId)
        {
            return _types.TryGetValue(nodeId, out var type) ? type : null;
        }

        public void AddMangledName(int nodeId, string name) => _mangledNames.Add(nodeId, name);

        public string GetNodeMangledName(int nodeId)
        {
            return _mangledNames.TryGetValue(nodeId, out var name) ? name : null;
        }
    }
}

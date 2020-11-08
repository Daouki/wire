namespace WireC.AST
{
    public interface INode
    {
        /// <summary>
        /// An identifier used to differentiate between nodes within an abstract syntax tree.
        /// It must be unique for each node of a tree.
        /// </summary>
        public int NodeId { get; }
    }
}

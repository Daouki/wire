namespace WireC.FrontEnd
{
    public class NodeIdGenerator
    {
        private int _nextId;

        public int GetNextId() => _nextId++;
    }
}

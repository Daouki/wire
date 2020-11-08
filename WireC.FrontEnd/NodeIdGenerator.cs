namespace WireC.FrontEnd
{
    public class NodeIdGenerator
    {
        private static int _nextId;

        public int GetNextId() => _nextId++;
    }
}

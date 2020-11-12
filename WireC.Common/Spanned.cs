namespace WireC.Common
{
    public class Spanned<T>
    {
        public Spanned(T node, SourceSpan span)
        {
            Node = node;
            Span = span;
        }

        public T Node { get; }
        public SourceSpan Span { get; }
    }
}

namespace JadeControls.EditorControl.Highlighting
{
    public interface IHighlighter
    {
        IHighlightedRange AddRange(int offset, int length);
        void RemoveRange(IHighlightedRange range);
        void Clear();
        void Redraw(IHighlightedRange range);
    }
}

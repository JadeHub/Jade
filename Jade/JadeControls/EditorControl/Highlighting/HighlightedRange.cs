using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;

namespace JadeControls.EditorControl.Highlighting
{
    public interface IHighlightedRange : ISegment
    {
        Color? BackgroundColour { get; set; }
        Color? ForegroundColour { get; set; }                
    }

    internal class HighlightedRange : TextSegment, IHighlightedRange
    {
        #region  Data

        private IHighlighter _highlighter;
        private Color? _background;
        private Color? _foreground;

        #endregion

        public HighlightedRange(IHighlighter highlighter, int startOffset, int length)
        {
            _highlighter = highlighter;
            StartOffset = startOffset;
            Length = length;
        }

        public Color? BackgroundColour
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    _highlighter.Redraw(this);
                }
            }
        }

        public Color? ForegroundColour  
        {
            get { return _foreground; }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    _highlighter.Redraw(this);
                }
            }
        }
    }
}

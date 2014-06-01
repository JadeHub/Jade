using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace JadeControls.EditorControl.Highlighting
{
    public class Highlighter : IHighlighter, IBackgroundRenderer, ITextViewConnect
    {        
        #region Data

        private TextSegmentCollection<HighlightedRange> _highlights;
        private JadeCore.ITextDocument _document;
        private readonly List<TextView> _textViews;

        #endregion

        #region Constructor

        public Highlighter(JadeCore.ITextDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            _document = doc;
            _highlights = new TextSegmentCollection<HighlightedRange>();
            _textViews = new List<TextView>();
        }

        #endregion

        #region IHighlighter implementation

        public IHighlightedRange AddRange(int offset, int length)
        {
            int textLength = _document.TextLength;
            if (offset < 0 || offset > textLength)
                throw new ArgumentOutOfRangeException("startOffset", offset, "Value must be between 0 and " + textLength);
            if (length < 0 || offset + length > textLength)
                throw new ArgumentOutOfRangeException("length", length, "length must not be negative and offset+length must not be after the end of the document");

            HighlightedRange result = new HighlightedRange(this, offset, length);
            _highlights.Add(result);
            return result;
        }

        public void RemoveRange(IHighlightedRange range)
        {
            if (!(range is HighlightedRange))
                throw new ArgumentException("range is not of type HighlightedRange");

            HighlightedRange hr = range as HighlightedRange;
            if (_highlights.Remove(hr))
            {
                Redraw(hr);
            }
        }

        public void Clear()
        {
            _highlights.Clear();
            foreach (var view in _textViews)
            {
                view.Redraw();
            }                
        }

        public void Redraw(IHighlightedRange segment)
        {
            foreach (var view in _textViews)
            {
                view.Redraw(segment, DispatcherPriority.Normal);
            }
        }
                
        #endregion

        #region IBackgroundRenderer

        public KnownLayer Layer 
        {
            get { return KnownLayer.Selection; }
        }

        /// <summary>
        /// Causes the background renderer to draw.
        /// </summary>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView.VisualLines.Count == 0)
                return;
            
            int viewStart = textView.VisualLines.First().FirstDocumentLine.Offset;
            int viewEnd = textView.VisualLines.Last().LastDocumentLine.EndOffset;
            foreach (IHighlightedRange hl in _highlights.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (hl.BackgroundColour != null)
                {
                    BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.AlignToWholePixels = true;
                    geoBuilder.CornerRadius = 3;
                    geoBuilder.AddSegment(textView, hl);
                    Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        Color color = hl.BackgroundColour.Value;
                        SolidColorBrush brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }

                if (hl.ForegroundColour != null)
                {
                    DrawUnderline(textView, drawingContext, hl, hl.ForegroundColour.Value);
                }
            }
        }

        #endregion

        #region Underlining

        private void DrawUnderline(TextView textView, DrawingContext drawingContext, ISegment segment, Color colour)
        {
            foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
            {
                Point startPoint = r.BottomLeft;
                Point endPoint = r.BottomRight;

                startPoint.Y += 1;
                endPoint.Y += 1;

                Pen usedPen = new Pen(new SolidColorBrush(colour), 1);
                usedPen.Freeze();
                double offset = 2.5;

                int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                StreamGeometry geometry = new StreamGeometry();

                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(startPoint, false, false);
                    ctx.PolyLineTo(CreateUnderlinePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                }

                geometry.Freeze();

                drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
            }
        }

        IEnumerable<Point> CreateUnderlinePoints(Point start, Point end, double offset, int count)
        {
            for (int i = 0; i < count; i++)
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
        }

        #endregion

        #region ITextViewConnect

        public void AddToTextView(TextView textView)
        {
            _textViews.Add(textView);
        }

        public void RemoveFromTextView(TextView textView)
        {
            _textViews.Remove(textView);
        }

        #endregion
    }
}

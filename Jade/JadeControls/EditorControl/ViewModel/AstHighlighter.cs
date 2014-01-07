using System.Windows.Media;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    public class ASTHighlighter
    {
        static string[] ColourValues = new string[] { 
        "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF", "#00FFFF", "#000000", 
        "#800000", "#008000", "#000080", "#808000", "#800080", "#008080", "#808080", 
        "#C00000", "#00C000", "#0000C0", "#C0C000", "#C000C0", "#00C0C0", "#C0C0C0", 
        "#400000", "#004000", "#000040", "#404000", "#400040", "#004040", "#404040", 
        "#200000", "#002000", "#000020", "#202000", "#200020", "#002020", "#202020", 
        "#600000", "#006000", "#000060", "#606000", "#600060", "#006060", "#606060", 
        "#A00000", "#00A000", "#0000A0", "#A0A000", "#A000A0", "#00A0A0", "#A0A0A0", 
        "#E00000", "#00E000", "#0000E0", "#E0E000", "#E000E0", "#00E0E0", "#E0E0E0", };

        private int _nextColour = 0;

        private Highlighting.IHighlighter _underliner;
        private string _path;

        public ASTHighlighter(Cursor topLevelCursor, Highlighting.IHighlighter underliner, string path)
        {
            _underliner = underliner;
            _path = System.IO.Path.GetFullPath(path);
            Highlight(topLevelCursor);
        }

        private void Highlight(Cursor c)
        {
            Highlighting.IHighlightedRange range = _underliner.AddRange(c.Extent.Start.Offset, c.Extent.Length);
            range.ForegroundColour = GetNextColour();

            foreach (Cursor child in c.Children)
            {
                if (child.Location.File == null)
                    continue;
                if(System.IO.Path.GetFullPath(child.Location.File.Name) == _path)
                    Highlight(child);
            }
        }

        private Color GetNextColour()
        {
            Color c = (Color)ColorConverter.ConvertFromString(ColourValues[_nextColour]);
            _nextColour++;

            if (_nextColour == ColourValues.Length) 
                _nextColour = 0;

            return c;
        }
    }
}

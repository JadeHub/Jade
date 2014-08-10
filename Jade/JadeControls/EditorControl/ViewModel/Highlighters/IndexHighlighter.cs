using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using JadeUtils.IO;
using CppCodeBrowser.Symbols;
using CppCodeBrowser.Symbols.FileMapping;

namespace JadeControls.EditorControl.ViewModel
{
    public class IndexHighlighter
    {
        private Highlighting.Highlighter _underliner;

        public IndexHighlighter(Highlighting.Highlighter underliner)
        {
            _underliner = underliner;
        }

        public void SetMap(IFileMap map)
        {
            foreach(Tuple<int, int, ISymbol> mapping in map.Symbols)
            {
                Highlighting.IHighlightedRange range = _underliner.AddRange(mapping.Item1, mapping.Item2 - mapping.Item1);
                
                if(mapping.Item3 is IDeclaration)
                {
                    range.ForegroundColour = Colors.Red;
                }
                else
                {
                    range.ForegroundColour = Colors.Blue;
                }
            }
        }

        public ICSharpCode.AvalonEdit.Rendering.IBackgroundRenderer Renderer
        {
            get { return _underliner; }
        }
    }
}

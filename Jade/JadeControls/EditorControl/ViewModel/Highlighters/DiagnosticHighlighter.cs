using LibClang;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Media;

namespace JadeControls.EditorControl.ViewModel
{
    public class DiagnosticHighlighter
    {
        private Highlighting.Highlighter _highlighter;
        private CppCodeBrowser.IProjectFile _projectItem;

        public DiagnosticHighlighter(Highlighting.Highlighter highlighter)
        {
            _highlighter = highlighter;
        }

        public CppCodeBrowser.IProjectFile ProjectItem
        {
            set
            {
                _projectItem = value;
                HighlightDiagnostics();
            }
        }

        public ICSharpCode.AvalonEdit.Rendering.IBackgroundRenderer Renderer
        {
            get { return _highlighter; }
        }

        private IEnumerable<CppCodeBrowser.ISourceFile> GetSources()
        {
            if (_projectItem is CppCodeBrowser.ISourceFile)
            {
                List<CppCodeBrowser.ISourceFile> temp = new List<CppCodeBrowser.ISourceFile>();
                temp.Add(_projectItem as CppCodeBrowser.ISourceFile);
                return temp;
            }
            else if (_projectItem is CppCodeBrowser.IHeaderFile)
            {
                return (_projectItem as CppCodeBrowser.IHeaderFile).SourceFiles;
            }
            Debug.Assert(false);
            return null;
        }

        private void HighlightDiagnostics()
        {
            _highlighter.Clear();

            if (_projectItem == null) return;

            foreach (Diagnostic d in _projectItem.Diagnostics)
                HighlightDiagnostic(d);
        }

        private SourceRange GetHighlightRange(Diagnostic diagnostic)
        {            
            Cursor cursor = diagnostic.LocationCursor;
            if (cursor == null || cursor.Extent == null) return null;

            TokenSet tokens = cursor.Extent.Tokens;
            if (tokens == null)
                return null;

            Token tok = tokens.GetTokenAtOffset(diagnostic.Location.Offset);
            return tok == null ? null : tok.Extent;
        }                    

        private void HighlightDiagnostic(Diagnostic diagnostic)
        {
            SourceRange extent = GetHighlightRange(diagnostic);
            if (extent == null)
                return;
            if (diagnostic.DiagnosticSeverity == Diagnostic.Severity.Error ||
                diagnostic.DiagnosticSeverity == Diagnostic.Severity.Warning)
            {
                Highlighting.IHighlightedRange hr = _highlighter.AddRange(extent.Start.Offset, extent.Length);
                hr.ForegroundColour = diagnostic.DiagnosticSeverity == Diagnostic.Severity.Error ? Colors.Red : Colors.Blue;
            }
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    public class DiagnosticHighlighter
    {
        private Highlighting.IHighlighter _underliner;
        private CppCodeBrowser.IProjectItem _itemBrowser;

        public DiagnosticHighlighter(CppCodeBrowser.IProjectItem itemBrowser, Highlighting.IHighlighter underliner)
        {
            _underliner = underliner;
            _itemBrowser = itemBrowser;
            HighlightDiagnostics();
        }

        private void HighlightDiagnostics()
        {
            _underliner.Clear();

            foreach (TranslationUnit tu in _itemBrowser.TranslationUnits)
            {
                foreach (Diagnostic d in tu.Diagnostics)
                {
                    if (System.IO.Path.GetFullPath(d.Location.File.Name) == _itemBrowser.Path)
                    {
                        HighlightDiagnostic(tu, d);
                    }
                }
            }

            /*foreach (Diagnostic diag in diagnostics)
            {
                HighlightDiagnostic(diag);
            }*/
        }

        private SourceRange GetHighlightRange(TranslationUnit tu, Diagnostic diagnostic)
        {
            
            Cursor cursor = tu.GetCursorAt(diagnostic.Location);
            if (cursor == null || cursor.Extent == null) return null;

            TokenSet tokens = TokenSet.Create(tu, cursor.Extent);
            if (tokens == null)
                return null;

            Token tok = tokens.GetTokenAtOffset(diagnostic.Location.Offset);
            return tok == null ? null : tok.Extent;
        }                    

        private void HighlightDiagnostic(TranslationUnit tu, Diagnostic diagnostic)
        {
            SourceRange extent = GetHighlightRange(tu, diagnostic);
            if (extent == null)
                return;
            if (diagnostic.DiagnosticSeverity == Diagnostic.Severity.Error ||
                diagnostic.DiagnosticSeverity == Diagnostic.Severity.Warning)
            {
                Highlighting.IHighlightedRange hr = _underliner.AddRange(extent.Start.Offset, extent.Length);
                hr.ForegroundColour = diagnostic.DiagnosticSeverity == Diagnostic.Severity.Error ? Colors.Red : Colors.Blue;
            }
        }
    }
}

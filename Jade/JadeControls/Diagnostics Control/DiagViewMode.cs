using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.DiagnosticsControl
{
    public class DiagViewMode
    {
        private LibClang.Diagnostic _diag;

        public DiagViewMode(LibClang.Diagnostic diag)
        {
            _diag = diag;
        }

        public string Text
        {
            get { return _diag.Spelling; }
        }

        public bool IsWarning
        {
            get { return _diag.DiagnosticSeverity == LibClang.Diagnostic.Severity.Warning; }
        }

        public bool IsError
        {
            get { return _diag.DiagnosticSeverity == LibClang.Diagnostic.Severity.Error || _diag.DiagnosticSeverity == LibClang.Diagnostic.Severity.Fatal; }
        }
    }
}

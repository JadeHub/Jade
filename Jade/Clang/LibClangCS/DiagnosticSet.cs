using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    internal class DiagnosticSet : IDisposable
    {
        #region Data

        internal IntPtr Handle { get; private set; }
        private HashSet<Diagnostic> _diags;

        internal DiagnosticSet(IntPtr handle, ITranslationUnitItemFactory itemFactory)
        {
            Handle = handle;
            _diags = new HashSet<Diagnostic>();
            for (uint i = 0; i < Library.clang_getNumDiagnosticsInSet(Handle); i++)
            {
                _diags.Add(new Diagnostic(Library.clang_getDiagnosticInSet(Handle, i), itemFactory));
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                Library.clang_disposeDiagnosticSet(Handle);
                Handle = IntPtr.Zero;
                _diags = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Properties

        public IEnumerable<Diagnostic> Diagnostics { get { return _diags; } }

        public int Count { get { return _diags.Count; } }

        #endregion
    }
}

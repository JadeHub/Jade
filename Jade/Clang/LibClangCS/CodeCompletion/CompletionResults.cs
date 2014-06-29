using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    public class Results : IDisposable
    {
        private List<Result> _results;
        private unsafe CodeCompletion.Library.CXCodeCompleteResults* _handleToDispose;
        private CodeCompletion.Library.CXCodeCompleteResults _handle;
        
        internal unsafe Results(CodeCompletion.Library.CXCodeCompleteResults* handle, TranslationUnit tu)
        {
            Int64 ii = Library.clang_codeCompleteGetContexts(handle);

            _handleToDispose = handle;
            _handle = *handle;
            _results = new List<Result>();

            bool print = _handle.NumberResults <= 100;
            for(uint i = 0;i < _handle.NumberResults; i++)
            {
                //Library.CXCompletionResult r = _handle.Results[i];
                Result r = new Result(_handle.Results[i]);
                _results.Add(r);
                if(print)
                    System.Diagnostics.Debug.WriteLine(r);
            }
            if(!print)
                System.Diagnostics.Debug.WriteLine("too many results");

          /*  List<LibClang.Diagnostic> diags = new List<Diagnostic>();
            for(uint d = 0; d < Library.clang_codeCompleteGetNumDiagnostics(_handleToDispose); d++)
            {
                Diagnostic diag = new Diagnostic(Library.clang_codeCompleteGetDiagnostic(handle, d), tu.ItemFactory);
                
                System.Diagnostics.Debug.WriteLine(diag + " " + diag.Location);
            }*/
        }

        private unsafe void DisposeHandle()
        {
            if (_handleToDispose != null)
            {
                Library.clang_disposeCodeCompleteResults(_handleToDispose);
                _handleToDispose = null;
            }
        }

        public void Dispose()
        {
            DisposeHandle();
        }

        public IEnumerable<Result> Items { get { return _results; } }

    }
}

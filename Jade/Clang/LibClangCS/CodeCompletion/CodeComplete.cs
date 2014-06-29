using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    

    internal static class CodeComplete
    {
        static unsafe internal Results CompleteAt(TranslationUnit tu, string fileName, int line, int col, LibClang.Library.UnsavedFile[] unsaved)
        {
            uint options = CodeCompletion.Library.clang_defaultCodeCompleteOptions();
            CodeCompletion.Library.CXCodeCompleteResults* results = CodeCompletion.Library.clang_codeCompleteAt(tu.Handle, fileName, (uint)line, (uint)col,
                                                                                            unsaved.Length > 0 ? unsaved : null,
                                                                                            (uint)unsaved.Length,
                                                                                            3);/*
                                                                                            CodeCompletion.Library.clang_defaultCodeCompleteOptions());*/
            Int64 p = Library.clang_codeCompleteGetContexts(results);
            if (results != null && results->NumberResults > 0)
            {
                Results rs = new Results(results, tu);
                return rs;
            }
            return null;
        }
    }
}

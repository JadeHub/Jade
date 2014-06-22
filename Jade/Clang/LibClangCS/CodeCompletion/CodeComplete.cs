using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    

    internal static class CodeComplete
    {
        static private void PrintChunks(Result r, ChunkKind k)
        {
            System.Diagnostics.Debug.WriteLine("Printing Chunks of type " + k.ToString());

            foreach (ResultChunk rc in r.GetChunks(k))
            {
                System.Diagnostics.Debug.WriteLine(rc.Text);
            }
        }

        static unsafe internal Results CompleteAt(TranslationUnit tu, string fileName, int line, int col, LibClang.Library.UnsavedFile[] unsaved)
        {
            uint options = CodeCompletion.Library.clang_defaultCodeCompleteOptions();
            CodeCompletion.Library.CXCodeCompleteResults* results = CodeCompletion.Library.clang_codeCompleteAt(tu.Handle, fileName, (uint)line, (uint)col,
                                                                                            unsaved.Length > 0 ? unsaved : null,
                                                                                            (uint)unsaved.Length,
                                                                                            (uint)CompletionContext.DotMemberAccess);/*
                                                                                            CodeCompletion.Library.clang_defaultCodeCompleteOptions());*/
            Int64 p = Library.clang_codeCompleteGetContexts(results);
            if (results != null && results->NumberResults > 0)
            {
                Results rs = new Results(*results);

                CodeCompletion.Library.clang_disposeCodeCompleteResults(results);
                results = null;

                foreach(Result r in rs.Items)
                {
                    System.Diagnostics.Debug.WriteLine("Printing result " + r.ToString());
                    /*PrintChunks(r, ChunkKind.TypedText);
                    PrintChunks(r, ChunkKind.Informative);
                    PrintChunks(r, ChunkKind.Placeholder);
                    PrintChunks(r, ChunkKind.Optional);*/

                    foreach(ResultChunk c in r.Chunks)
                    {
                        System.Diagnostics.Debug.WriteLine("Chunk Type: " + c.Kind.ToString() + " Text: " + c.Text);
                    }
                }
                
                return rs;
            }
            return null;
        }
    }
}

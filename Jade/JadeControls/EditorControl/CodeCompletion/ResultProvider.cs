using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using JadeUtils.IO;

namespace JadeControls.EditorControl.CodeCompletion
{
    public interface IResultProvider
    {
        //IEnumerable<ICompletionData> GetResults(string file, int line, int column);
        ResultSet GetResults(string file, int line, int column, CompletionSelection selection);
    }

    public class ResultProvider : IResultProvider
    {
        private CppCodeBrowser.IProjectIndex _index;
        private FilePath _sourceFile;
        private CppCodeBrowser.IUnsavedFileProvider _unsavedFiles;

        public ResultProvider(FilePath sourceFile, CppCodeBrowser.IProjectIndex index, CppCodeBrowser.IUnsavedFileProvider unsavedFiles)
        {
            _index = index;
            _sourceFile = sourceFile;
            _unsavedFiles = unsavedFiles;
        }

        public ResultSet GetResults(string file, int line, int column, CompletionSelection selection)
        {
            ResultSet resultSet = new ResultSet();
            CppCodeBrowser.ISourceFile sf = _index.FindSourceFile(_sourceFile);
            if (sf == null) return null;
            
            LibClang.CodeCompletion.Results results = sf.TranslationUnit.CodeCompleteAt(file, line, column, _unsavedFiles.GetUnsavedFiles());
            if (results == null || results.Items.Count() == 0) return null;

            foreach (LibClang.CodeCompletion.Result r in results.Items.OrderBy(item => item.CompletionPriority))
            {
                resultSet.Add(new CompletionData(r, selection));
            }

            return resultSet;
        }
        /*
        public IEnumerable<ICompletionData> GetResults(string file, int line, int column)
        {
            CppCodeBrowser.ISourceFile sf = _index.FindSourceFile(_sourceFile);
            if (sf != null)
            {
                LibClang.CodeCompletion.Results results = sf.TranslationUnit.CodeCompleteAt(file, line, column, _unsavedFiles.GetUnsavedFiles());
                if (results != null)
                {
                    foreach (LibClang.CodeCompletion.Result r in results.Items.OrderBy(item => item.TypedChunk.Text))
                    {
                        yield return new CompletionData(r);
                    }
                }
            }
        }*/
    }
}

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace JadeControls.EditorControl.CodeCompletion
{
    public class CodeCompletion
    {
        private TextArea _textArea;
        private CompletionWindow _completionWindow;
        CppCodeBrowser.IUnsavedFileProvider _unsavedFiles;

        public CodeCompletion(TextArea textArea, CppCodeBrowser.IUnsavedFileProvider unsavedFiles)
        {
            _textArea = textArea;
            _unsavedFiles = unsavedFiles;
        }

        public void DoCompletion(LibClang.TranslationUnit tu, string file, int line, int col)
        {
            Debug.Assert(_completionWindow == null);
            LibClang.CodeCompletion.Results results = tu.CodeCompleteAt(file, line, col, _unsavedFiles.GetUnsavedFiles());
            if (results == null)
                return;

            _completionWindow = new CompletionWindow(_textArea);
            IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;
            foreach(LibClang.CodeCompletion.Result r in results.Items.OrderBy(item => item.TypedChunk.Text))
            {
                data.Add(new CompletionData(r));
            }
            _completionWindow.Show();
            _completionWindow.Closed += (o, args) => _completionWindow = null;
        }
    }
}

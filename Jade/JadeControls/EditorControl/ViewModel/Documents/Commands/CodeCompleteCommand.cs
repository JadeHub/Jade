 using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class CodeCompleteCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;
        
        public CodeCompleteCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
            : base(vm)
        {
            ViewModel.CaretOffsetChanged += ViewModelCaretOffsetChanged;
            _path = path;
            _index = index;
            RaiseCanExecuteChangedEvent();
        }

        private void ViewModelCaretOffsetChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChangedEvent();
        }

        protected override bool CanExecute()
        {
            return CodeCompletion != null;
        }

        protected override void Execute()
        {         
            /*
            int startOffset = ViewModel.CaretOffset;
            while (startOffset > 0 && char.IsLetterOrDigit(ViewModel.TextDocument.Text[startOffset-1]))
                startOffset--;
            int line = 0;
            int col = 0;
            ViewModel.TextDocument.GetLineAndColumnForOffset(startOffset, out line, out col);
            string triggerWord = ViewModel.TextDocument.Text.Substring(startOffset, ViewModel.CaretOffset - startOffset);
            */
         //   CppCodeBrowser.ISourceFile sf = _index.FindSourceFile(_path);
          //  if (sf == null) return;
            /*
            int offset;
            string triggerWord = CodeCompletion.ExtractTriggerWord(ViewModel.CaretOffset, out offset);
            */
            CodeCompletion.BeginSelection(ViewModel.CaretOffset);//, triggerWord);
        }

        public CodeCompletion.CompletionEngine CodeCompletion
        {
            get;
            set;
        }
    }
}

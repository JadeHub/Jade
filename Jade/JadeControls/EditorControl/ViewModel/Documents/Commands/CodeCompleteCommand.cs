using System;
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
            return true;
        }

        protected override void Execute()
        {
            CppCodeBrowser.ISourceFile sf = _index.FindSourceFile(_path);
            if (sf == null) return;

            CppCodeBrowser.IUnsavedFileProvider unsavedProvider = JadeCore.Services.Provider.EditorController;

            LibClang.CodeCompletion.Results r = sf.TranslationUnit.CodeCompleteAt(_path.Str, ViewModel.CaretLine, ViewModel.CaretColumn, unsavedProvider.GetUnsavedFiles());

            //ViewModel.CaretLine 
        }
    }
}

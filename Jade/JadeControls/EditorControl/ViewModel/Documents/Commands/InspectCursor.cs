using System;
using JadeUtils.IO;


namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class InspectCursorCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;

        public InspectCursorCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
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
            return GetCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset)) != null;
        }

        protected override void Execute()
        {
            LibClang.Cursor c = GetCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset));
            if (c != null)
            {
                JadeCore.Services.Provider.CommandHandler.OnDisplayCursorInspector(c);
            }
        }

        private LibClang.Cursor GetCursorAt(CppCodeBrowser.ICodeLocation loc)
        {
            CppCodeBrowser.IProjectFile fileIndex = _index.FindProjectItem(loc.Path);
            if (fileIndex == null)
                return null;
            if (fileIndex is CppCodeBrowser.ISourceFile)
                return (fileIndex as CppCodeBrowser.ISourceFile).GetCursorAt(loc);

            CppCodeBrowser.IHeaderFile header = fileIndex as CppCodeBrowser.IHeaderFile;
            foreach (CppCodeBrowser.ISourceFile sf in header.SourceFiles)
            {
                LibClang.Cursor c = sf.GetCursorAt(loc);
                if (c != null)
                    return c;
            }
            return null;
        }
    }
}

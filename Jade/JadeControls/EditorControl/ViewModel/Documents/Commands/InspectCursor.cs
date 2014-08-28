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
            CppCodeBrowser.Symbols.ISymbol symbol = _index.FileSymbolMaps.Lookup(_path, ViewModel.CaretOffset);
            if (symbol == null) return null;

            return symbol.Cursor;
        }
    }
}

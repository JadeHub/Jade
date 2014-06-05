using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;
using JadeCore.CppSymbols;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class InspectSymbolCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;
            
        public InspectSymbolCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
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
            LibClang.Cursor c = CppCodeBrowser.BrowsingUtils.GetDefinitionOfCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset), _index);
            return (c != null && JadeCore.Services.Provider.SymbolCursorFactory.CanCreate(c));
        }

        protected override void Execute()
        {
            LibClang.Cursor c = CppCodeBrowser.BrowsingUtils.GetDefinitionOfCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset), _index);
            if(c != null && JadeCore.Services.Provider.SymbolCursorFactory.CanCreate(c))
            {
                JadeCore.Services.Provider.CommandHandler.OnDisplaySymbolInspector(JadeCore.Services.Provider.SymbolCursorFactory.Create(c));
            }
        }
    }
}

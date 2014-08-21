using System;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class DebugCursorCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;

        public DebugCursorCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
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
            CppCodeBrowser.ISourceFile source = _index.FindSourceFile(_path);
            if (source == null) return;

            LibClang.Cursor c = source.TranslationUnit.GetCursorAt(_path.Str, ViewModel.CaretOffset);
            foreach(var t in c.ArgumentCursors)
            {
                System.Diagnostics.Debug.WriteLine(t);
            }
        }
    }

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
            return true;
            //return GetCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset)) != null;
            /*LibClang.Cursor c = CppCodeBrowser.BrowsingUtils.GetDefinitionOfCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset), _index);
            return (c != null && JadeCore.Services.Provider.SymbolCursorFactory.CanCreate(c));*/
        }

        protected override void Execute()
        {
            CppCodeBrowser.Symbols.FileMapping.IFileMap map = _index.FileSymbolMaps.GetMap(_path);
            if (map == null) return;

            CppCodeBrowser.Symbols.ISymbol symbol = map.Get(ViewModel.CaretOffset);
            if (symbol == null) return;

            if(symbol is CppCodeBrowser.Symbols.IDeclaration)
                JadeCore.Services.Provider.CommandHandler.OnDisplaySymbolInspector(symbol as CppCodeBrowser.Symbols.IDeclaration);

            /*
            LibClang.Cursor c = CppCodeBrowser.BrowsingUtils.GetDefinitionOfCursorAt(new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset), _index);
            if(c != null && JadeCore.Services.Provider.SymbolCursorFactory.CanCreateKind(c.Kind))
            {
                JadeCore.Services.Provider.CommandHandler.OnDisplaySymbolInspector(JadeCore.Services.Provider.SymbolCursorFactory.Create(c));
            }*/
        }
        /*
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
        }*/
    }
}

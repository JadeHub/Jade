using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeCore;
using JadeUtils.IO;
using LibClang;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class FindAllReferences : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.IProjectIndex _index;

        public FindAllReferences(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
            : base(vm)
        {
            ViewModel.CaretOffsetChanged += ViewModelCaretOffsetChanged;
            _path = path;
            _index = index;            
        }

        private void ViewModelCaretOffsetChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChangedEvent();
        }

        protected override bool CanExecute()
        {
            CppCodeBrowser.Symbols.ISymbol symbol = _index.FileSymbolMaps.Lookup(_path, ViewModel.CaretOffset);
            return symbol != null;
            /*
            CppCodeBrowser.IProjectFile fileIndex = _index.FindProjectItem(_path);
            if (fileIndex == null) return false;

            if(fileIndex is CppCodeBrowser.ISourceFile)
            {
                LibClang.Cursor c = (fileIndex as CppCodeBrowser.ISourceFile).TranslationUnit.GetCursorAt(_path.Str, ViewModel.CaretOffset);
                return c != null && c.Kind != CursorKind.NoDeclFound;
            }

            if(fileIndex is CppCodeBrowser.IHeaderFile)
            {
                CppCodeBrowser.IHeaderFile header = fileIndex as CppCodeBrowser.IHeaderFile;
                foreach(CppCodeBrowser.ISourceFile sf in header.SourceFiles)
                {
                    LibClang.Cursor c = sf.TranslationUnit.GetCursorAt(_path.Str, ViewModel.CaretOffset);
                    return c != null && c.Kind != CursorKind.NoDeclFound;
                }
            }
            Debug.Assert(false);
            return true;*/
        }

        protected override void Execute()
        {
            Debug.Assert(_index != null);

            CppCodeBrowser.CodeLocation location = new CppCodeBrowser.CodeLocation(_path.Str, ViewModel.CaretOffset);
            JadeCore.Search.ISearch search = new JadeCore.Search.FindAllReferencesSearch(_index, location);
            Services.Provider.SearchController.RegisterSearch(search);
            search.Start();
        }
    }
}

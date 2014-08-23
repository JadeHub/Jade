using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using JadeUtils.IO;
using CppCodeBrowser.Symbols;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class SourceFileJumpToCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.JumpToBrowser _jumpToBrowser;
        private CppCodeBrowser.IProjectIndex _index;
        //private CppCodeBrowser.IProjectFile _indexItem;

        public SourceFileJumpToCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
            : base(vm)
        {
            ViewModel.CaretOffsetChanged += ViewModelCaretOffsetChanged;

            _path = path;
            _index = index;
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(_index);
            _index.ItemUpdated += OnIndexItemUpdated;
           // _indexItem = _index.FindProjectItem(_path);
            RaiseCanExecuteChangedEvent();
        }

        private void ViewModelCaretOffsetChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChangedEvent();
        }

        private void OnIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
          //  _indexItem = _index.FindProjectItem(_path);
        }

        protected override bool CanExecute()
        {
            return true;
          /*  _indexItem = _index.FindProjectItem(_path);
            bool b = JumpTo(ViewModel.CaretOffset) != null;
            Debug.WriteLine("CE " + b.ToString());
            return b;*/
        }

        protected override void Execute()
        {
            CppCodeBrowser.ICodeLocation result = JumpTo(ViewModel.CaretOffset);

            if (result != null)
            {
                JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(result, true, true));
            }
        }

        private IList<LibClang.Cursor> GetSourceCursors(int offset)
        {
            List<LibClang.Cursor> result = new List<LibClang.Cursor>();

            CppCodeBrowser.Symbols.ISymbol symbol = _index.FileSymbolMaps.Lookup(_path, ViewModel.CaretOffset);
            if (symbol != null)
                result.Add(symbol.Cursor);
            return result;

            /*

            if(_indexItem is CppCodeBrowser.ISourceFile)
            {
                LibClang.Cursor cur = (_indexItem as CppCodeBrowser.ISourceFile).GetCursorAt(_path, offset);
                if (cur != null && CanJumpTo(cur, offset))
                    result.Add(cur);
            }
            else if(_indexItem is CppCodeBrowser.IHeaderFile)
            {
                foreach(CppCodeBrowser.ISourceFile sf in ((_indexItem as CppCodeBrowser.IHeaderFile).SourceFiles))
                {
                    LibClang.Cursor cur = sf.GetCursorAt(_path, offset);
                    if (cur != null && CanJumpTo(cur, offset))
                        result.Add(cur);
                }
            }*/

          //  return result;
        }

        private CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            CppCodeBrowser.Symbols.ISymbol symbol = _index.FileSymbolMaps.Lookup(_path, ViewModel.CaretOffset);
            if (symbol == null)
                return null;

            if(symbol is CppCodeBrowser.Symbols.IReference)
            {
                return (symbol as CppCodeBrowser.Symbols.IReference).Declaration.Location;
            }
            else if(symbol is IDeclaration)
            {
                if (symbol is IHasDefinition)
                {
                    IDeclaration decl = symbol as IDeclaration;
                    IHasDefinition s = symbol as IHasDefinition;

                    if(decl.Location.Path == _path && offset >= decl.Location.Offset && offset <= decl.Location.Offset + decl.SpellingLength)
                    {
                        //we are on the declaration, go to the definition
                        if (s.HasDefinitionCursor)
                            return new CppCodeBrowser.CodeLocation(s.DefinitionCursor.Location);
                    }
                    else
                    {
                        //we are on the definition, go to the declaration
                        return decl.Location;
                    }                    
                }
            }
            return null;
        }

        private static bool CanJumpTo(LibClang.Cursor cursor, int offset)
        {
            if (cursor.Extent == null)
                return false;

            if (cursor.Kind == LibClang.CursorKind.InclusionDirective ||
                cursor.Kind == LibClang.CursorKind.CXXMethod) // Added CXXMethod to improve usage with operator over loads eg 'operator=' which are almost entirely TokenKind.Keyword not TokenKind.Identifier             
                return true;

            LibClang.Token tok = cursor.Extent.GetTokenAtOffset(offset);
            return (tok != null && tok.Kind == LibClang.TokenKind.Identifier);
        }
    }
}

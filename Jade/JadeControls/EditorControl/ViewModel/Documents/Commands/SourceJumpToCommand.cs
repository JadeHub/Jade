﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public class SourceFileJumpToCommand : EditorCommand
    {
        private FilePath _path;
        private CppCodeBrowser.JumpToBrowser _jumpToBrowser;
        private CppCodeBrowser.IProjectIndex _index;
        private CppCodeBrowser.IProjectFile _indexItem;

        public SourceFileJumpToCommand(DocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
            : base(vm)
        {
            ViewModel.CaretOffsetChanged += ViewModelCaretOffsetChanged;

            _path = path;
            _index = index;
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(_index);
            _index.ItemUpdated += OnIndexItemUpdated;
            _indexItem = _index.FindProjectItem(_path);
            RaiseCanExecuteChangedEvent();
        }

        private void ViewModelCaretOffsetChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChangedEvent();
        }

        private void OnIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
       //     if (path != _path)
           //     return;
            _indexItem = _index.FindProjectItem(_path);
        }

        protected override bool CanExecute()
        {
            return JumpTo(ViewModel.CaretOffset) != null;
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
            }

            return result;
        }

        private CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            if (_indexItem == null) return null;

            /*CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(_path.Str, offset);            
            LibClang.Cursor cur = _indexItem.GetCursorAt(location.Path, location.Offset);

            if (cur == null)
                return null;

            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>();
            cursors.Add(cur);
            */
            HashSet<CppCodeBrowser.ICodeLocation> results = new HashSet<CppCodeBrowser.ICodeLocation>();
            IList<LibClang.Cursor> cursors = GetSourceCursors(offset);

            _jumpToBrowser.BrowseFrom(cursors,
                delegate(CppCodeBrowser.ICodeLocation result)
                {
                    results.Add(result);
                    return true;
                });

            return results.Count() > 0 ? results.First() : null;
        }

        private static bool CanJumpTo(LibClang.Cursor cursor, int offset)
        {
            if (cursor.Extent == null)
                return false;

            if (cursor.Extent.Tokens == null)
                return false;

            if (cursor.Kind == LibClang.CursorKind.InclusionDirective)
                return true;

            LibClang.Token tok = cursor.Extent.Tokens.GetTokenAtOffset(offset);
            return (tok != null && tok.Kind == LibClang.TokenKind.Identifier);
        }
    }
}

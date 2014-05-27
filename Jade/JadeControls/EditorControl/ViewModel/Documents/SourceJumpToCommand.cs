using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class SourceFileJumpToCommand : EditorCommand<int>
    {
        private FilePath _path;
        private CppCodeBrowser.JumpToBrowser _jumpToBrowser;
        private CppCodeBrowser.IProjectIndex _index;
        private CppCodeBrowser.IProjectFile _indexItem;

        public SourceFileJumpToCommand(SourceDocumentViewModel vm, FilePath path, CppCodeBrowser.IProjectIndex index)
        {
            vm.CaretOffsetChanged += ViewModelCaretOffsetChanged;

            _path = path;
            _index = index;
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(_index);
            index.ItemUpdated += OnIndexItemUpdated;
            RaiseCanExecuteChangedEvent();
        }

        private void ViewModelCaretOffsetChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChangedEvent();
        }

        private void OnIndexItemUpdated(JadeUtils.IO.FilePath path)
        {
            if (path != _path)
                return;
            _indexItem = _index.FindProjectItem(path);
        }

        protected override bool CanExecute(int offset)
        {
            return JumpTo(offset) != null;
        }

        protected override void Execute(int offset)
        {
            CppCodeBrowser.ICodeLocation result = JumpTo(offset);

            if (result != null)
            {
                JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(result, true, true));
            }
        }

        private CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(_path.Str, offset);
            CppCodeBrowser.ISourceFile sourceFile = _indexItem as CppCodeBrowser.ISourceFile;

            LibClang.Cursor cur = sourceFile.TranslationUnit.GetCursorAt(location.Path.Str, location.Offset);

            if (cur == null)
                return null;

            List<LibClang.Cursor> cursors = new List<LibClang.Cursor>();
            cursors.Add(cur);

            HashSet<CppCodeBrowser.ICodeLocation> results = new HashSet<CppCodeBrowser.ICodeLocation>();

            _jumpToBrowser.BrowseFrom(from c in cursors where CanJumpTo(c, offset) select c,
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

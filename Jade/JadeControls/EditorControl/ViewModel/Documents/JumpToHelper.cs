using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class JumpToHelper
    {
        private FilePath _path;
        private CppCodeBrowser.JumpToBrowser _jumpToBrowser;

        public JumpToHelper(FilePath path, CppCodeBrowser.IProjectIndex index)
        {
            _path = path;
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(index);
        }

        public CppCodeBrowser.IProjectFile ProjectItemIndex
        {
            private get;
            set;
        }

        public CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            if (!(ProjectItemIndex is CppCodeBrowser.ISourceFile))
                return null;

            CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(_path.Str, offset);
            CppCodeBrowser.ISourceFile sourceFile = ProjectItemIndex as CppCodeBrowser.ISourceFile;

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

        private bool CanJumpTo(LibClang.Cursor cursor, int offset)
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

        public bool CanJumpTo(int offset)
        {
            return JumpTo(offset) != null;
        }
    }
}

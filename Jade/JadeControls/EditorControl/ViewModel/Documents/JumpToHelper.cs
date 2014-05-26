using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore;

namespace JadeControls.EditorControl.ViewModel
{
    public class JumpToHelper
    {
        private CppCodeBrowser.IProjectIndex _index;
        private IEditorDoc _doc;
        private CppCodeBrowser.ICodeBrowser _jumpToBrowser;

        public JumpToHelper(CppCodeBrowser.IProjectIndex index, IEditorDoc doc)
        {
            _index = index;
            _doc = doc;
            _jumpToBrowser = new CppCodeBrowser.JumpToBrowser(_index);
        }

        public CppCodeBrowser.IProjectFile ProjectItemIndex
        {
            get;
            set;
        }

        public CppCodeBrowser.ICodeLocation JumpTo(int offset)
        {
            if (!(ProjectItemIndex is CppCodeBrowser.ISourceFile))
                return null;

            CppCodeBrowser.ICodeLocation location = new CppCodeBrowser.CodeLocation(_doc.File.Path.Str, offset);
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

using JadeUtils;
using LibClang;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace CppCodeBrowser
{
    public class JumpToBrowser : ICodeBrowser
    {
        #region Data

        private IProjectIndex _index;

        #endregion

        #region Constructor

        public JumpToBrowser(IProjectIndex index)
        {
            _index = index;
        }

        #endregion

        #region ICodeBrowser

        public bool CanBrowseFrom(ICodeLocation loc)
        {
            return true;
        }

        public void BrowseFrom(IEnumerable<LibClang.Cursor> fromCursors, Func<ICodeLocation, bool> OnResult)
        {
            ArgChecking.ThrowIfNull(fromCursors, "fromCursors");
            
            foreach (LibClang.Cursor c in fromCursors)
            {
                ICodeLocation result = JumpTo(c);
                {
                    if (result!= null && OnResult(result) == false)
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private ICodeLocation JumpTo(Cursor c)
        {
            LibClang.Cursor result = null;

            if (c.Kind == CursorKind.InclusionDirective)
                return JumpToInclude(c);

            if (JumpToDeclaration(c))
            {
                //For constructor and method definitions we browse to the declaration
                result = FindDeclaration(c.SemanticParentCurosr, c.Usr);
            }
            else
            {
                if (c.CursorReferenced != null)
                    result = c.CursorReferenced;
                else if (c.Definition != null)
                    result = c.Definition;
                else
                    result = c.CanonicalCursor;
            }
            if (result == null)
                return null;
            return new CodeLocation(result.Location.File.Name, result.Location.Offset);
        }

        private ICodeLocation JumpToInclude(Cursor c)
        {
            Debug.Assert(c.Kind == CursorKind.InclusionDirective);

            return null;
        }

        private static bool JumpToDeclaration(LibClang.Cursor c)
        {
            return ((c.Kind == CursorKind.Constructor ||
                        c.Kind == CursorKind.Destructor ||
                        c.Kind == CursorKind.CXXMethod ||
                        c.Kind == CursorKind.FunctionDecl)
                && c.IsDefinition);
        }

        private Cursor FindDeclaration(Cursor parent, string usr)
        {
            Cursor result = null;
            parent.VisitChildren(delegate(Cursor cursor, Cursor p)
            {
                if (cursor.Usr == usr && cursor.IsDefinition == false)
                {
                    result = cursor;
                    return Cursor.ChildVisitResult.Break;
                }
                return Cursor.ChildVisitResult.Continue;
            });
            return result;
        }

        #endregion
    }
}

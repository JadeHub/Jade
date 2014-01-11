using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser
{
    public interface ICodeBrowser
    {
        IEnumerable<ICodeLocation> BrowseFrom(ICodeLocation location);
    }
    /*
    public class CodeBrowser : ICodeBrowser
    {
        public IProjectIndex Index
        {
            get;
            private set;
        }

        public CodeBrowser(IProjectIndex index)
        {
            Index = index;
        }

        public ICodeLocation[] GetReferencesToItemAt(ICodeLocation location)
        {
            return null;
        }

        public ICodeLocation JumpTo(ICodeLocation location)
        {
            LibClang.Cursor c = Index.GetCursorAt(location);
            if (c == null || !c.Valid)
                return null;

            LibClang.Cursor result = null;

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
    }*/
}

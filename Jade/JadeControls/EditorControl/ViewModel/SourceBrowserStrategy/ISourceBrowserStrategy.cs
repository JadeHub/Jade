using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClang;

namespace JadeControls.EditorControl.ViewModel
{
    /*
    public interface ISourceBrowserStrategy
    {
        /// <summary>
        /// Inspect the code at location and return the location to 'jump to'
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        //ICodeLocation JumpTo(ICodeLocation location);
        //bool CanJumpToAt(ICodeLocation location);
    }

    public class SourceBrowserStrategy : ISourceBrowserStrategy
    {
        public CppView.IProjectSourceIndex Index { get; private set; }

        public SourceBrowserStrategy(CppView.IProjectSourceIndex index)
        {
            Index = index;
        }

        public ICodeLocation JumpTo(ICodeLocation location)
        {
            LibClang.Cursor c = Index.GetCursorAt(location.Path, location.Offset);
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
            return result == null ? null : new CodeLocation(result.Location);
           
            //return null;
        }

        public bool CanJumpToAt(ICodeLocation location)
        {
            return JumpTo(location) != null;
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

    }
    */
    
}

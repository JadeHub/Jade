using System;
using LibClang;

namespace CppCodeBrowser
{
    public static class CursorKinds
    {
        static public bool IsDefinition(CursorKind k)
        {
            int kAsInt = (int)k;
            return (kAsInt >= (int)CursorKind.FirstDecl && kAsInt <= (int)CursorKind.LastDecl);
        }

        static public bool IsReference(CursorKind k)
        {
            int kAsInt = (int)k;
            return (kAsInt >= (int)CursorKind.FirstRef && kAsInt <= (int)CursorKind.LastRef);
        }

        static public bool IsClassStructEtc(CursorKind k)
        {
            return k == CursorKind.ClassDecl ||
                    k == CursorKind.StructDecl ||
                    k == CursorKind.ClassTemplate ||
                    k == CursorKind.ClassTemplatePartialSpecialization;

        }

        static public bool IsFunctionEtc(CursorKind k)
        {
            return k == CursorKind.CXXMethod ||
                    k == CursorKind.FunctionDecl ||
                    k == CursorKind.Constructor ||
                    k == CursorKind.Destructor;
                    //conv func?
        }
    }

    public static class BrowsingUtils
    {
        public static LibClang.Cursor GetDefinitionOfCursorAt(ICodeLocation location, IProjectIndex index)
        {
            IProjectFile fileIndex = index.FindProjectItem(location.Path);
            if (fileIndex == null) return null;

            Cursor c = null;
            if(fileIndex is ISourceFile)
            {
                c = GetDefinitionOf((fileIndex as ISourceFile).GetCursorAt(location));
            }
            else if(fileIndex is IHeaderFile)
            {
                c = GetDefinitionOfCursorAtHeaderLocation(fileIndex as IHeaderFile, location);
            }

            return c;
        }

        private static Cursor GetDefinitionOfCursorAtHeaderLocation(IHeaderFile header, ICodeLocation location)
        {
            foreach(ISourceFile sf in header.SourceFiles)
            {
                Cursor c = GetDefinitionOf(sf.GetCursorAt(location));
                if (c != null)
                    return c;                
            }
            return null;
        }

        private static Cursor GetDefinitionOf(Cursor c)
        {
            if (c == null) return c;

            if (c.IsDefinition)
                return c;

            return c.DefinitionCursor;
        }
    }
}

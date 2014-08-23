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

        static public bool IsExpression(CursorKind k)
        {
            int kAsInt = (int)k;
            return (kAsInt >= (int)CursorKind.FirstExpr && kAsInt <= (int)CursorKind.LastExpr);
        }

        static public bool IsStatement(CursorKind k)
        {
            int kAsInt = (int)k;
            return (kAsInt >= (int)CursorKind.FirstStmt && kAsInt <= (int)CursorKind.LastStmt);
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
}

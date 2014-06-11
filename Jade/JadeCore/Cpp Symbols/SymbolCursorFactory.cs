using System;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeCore.CppSymbols
{
    public class SymbolCursorFactory : ISymbolCursorFactory
    {
        public ClassDeclarationSymbol CreateClassDefinition(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.ClassDecl || c.Kind == LibClang.CursorKind.StructDecl);
            return new ClassDeclarationSymbol(c);
        }

        public MethodDeclarationSymbol CreateMethodDefinition(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.CXXMethod);
            return new MethodDeclarationSymbol(c);
        }

        public ConstructorDeclarationSymbol CreateConstructorDefinition(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.Constructor);
            return new ConstructorDeclarationSymbol(c);
        }

        public ISymbolCursor Create(LibClang.Cursor c)
        {
            if (c.Kind == LibClang.CursorKind.ClassDecl || c.Kind == LibClang.CursorKind.StructDecl)
                return CreateClassDefinition(c);
            else if (c.Kind == LibClang.CursorKind.CXXMethod)
                return CreateMethodDefinition(c);
            else if (c.Kind == LibClang.CursorKind.Constructor)
                return CreateConstructorDefinition(c);
            return null;
        }

        public bool CanCreate(LibClang.Cursor c)
        {
            return c.Kind == LibClang.CursorKind.ClassDecl || c.Kind == LibClang.CursorKind.StructDecl;
        }
    }
}

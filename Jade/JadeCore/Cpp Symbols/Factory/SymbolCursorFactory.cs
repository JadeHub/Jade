using System;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeCore.CppSymbols
{
    public class SymbolCursorFactory : ISymbolCursorFactory
    {
        public ClassDeclarationSymbol CreateClassDefinition(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.ClassDecl || c.Kind == LibClang.CursorKind.StructDecl || c.Kind == LibClang.CursorKind.ClassTemplate);
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

        public DestructorDeclarationSymbol CreateDestructorDefinition(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.Destructor);
            return new DestructorDeclarationSymbol(c);
        }

        public DataMemberDeclarationSymbol CreateDataMember(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.FieldDecl);
            return new DataMemberDeclarationSymbol(c);
        }

        public NamespaceSymbol CreateNamespace(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.Namespace);
            return new NamespaceSymbol(c);
        }

        public ISymbolCursor Create(LibClang.Cursor c)
        {
            if (c.Kind == LibClang.CursorKind.ClassDecl || c.Kind == LibClang.CursorKind.StructDecl || c.Kind == LibClang.CursorKind.ClassTemplate)
                return CreateClassDefinition(c);
            else if (c.Kind == LibClang.CursorKind.CXXMethod)
                return CreateMethodDefinition(c);
            else if (c.Kind == LibClang.CursorKind.Constructor)
                return CreateConstructorDefinition(c);
            else if (c.Kind == LibClang.CursorKind.Destructor)
                return CreateDestructorDefinition(c);
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
                return CreateDataMember(c);
            else if (c.Kind == LibClang.CursorKind.Namespace)
                return CreateNamespace(c);
            return null;
        }

        public bool CanCreate(LibClang.Cursor c)
        {
            return c.Kind == LibClang.CursorKind.ClassDecl ||
                    c.Kind == LibClang.CursorKind.StructDecl ||
                    c.Kind == LibClang.CursorKind.CXXMethod ||
                    c.Kind == LibClang.CursorKind.Constructor ||
                    c.Kind == LibClang.CursorKind.Destructor ||
                    c.Kind == LibClang.CursorKind.FieldDecl ||
                    c.Kind == LibClang.CursorKind.ClassTemplate ||
                    c.Kind == LibClang.CursorKind.Namespace;


        }
    }
}

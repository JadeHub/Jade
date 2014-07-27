using System;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeCore.CppSymbols2
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

        public FunctionDeclSymbol CreateFunctionDeclaration(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.FunctionDecl);
            return new FunctionDeclSymbol(c);
        }

        public VariableDeclSymbol CreateVariableDeclaration(LibClang.Cursor c)
        {
            Debug.Assert(c.Kind == LibClang.CursorKind.VarDecl);
            return new VariableDeclSymbol(c);
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
            else if (c.Kind == LibClang.CursorKind.VarDecl)
                return CreateVariableDeclaration(c);
            else if (c.Kind == LibClang.CursorKind.FunctionDecl)
                return CreateFunctionDeclaration(c);
            return null;
        }

        public bool CanCreateKind(LibClang.CursorKind k)
        {
            return k == LibClang.CursorKind.ClassDecl ||
                    k == LibClang.CursorKind.StructDecl ||
                    k == LibClang.CursorKind.CXXMethod ||
                    k == LibClang.CursorKind.Constructor ||
                    k == LibClang.CursorKind.Destructor ||
                    k == LibClang.CursorKind.FieldDecl ||
                    k == LibClang.CursorKind.ClassTemplate ||
                    k == LibClang.CursorKind.Namespace ||
                    k == LibClang.CursorKind.FunctionDecl ||
                    k == LibClang.CursorKind.VarDecl;
        }
    }
}

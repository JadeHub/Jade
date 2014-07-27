using System;
using System.Collections.Generic;
using JadeCore.CppSymbols2;

namespace JadeControls.ContextTool
{
    public class Factory
    {
        static public ITreeItem MakeTreeItem(ITreeItem parent, LibClang.Cursor c)
        {
            ISymbolCursor symbol = JadeCore.Services.Provider.SymbolCursorFactory.Create(c);
/*
            if (symbol is NamespaceSymbol)
                return new NamespaceViewModel(symbol as NamespaceSymbol);

            if (symbol is ClassDeclarationSymbol)
                return new ClassViewModel(symbol as ClassDeclarationSymbol);
            */
            return new CursorViewModel(parent, c);
        }

        static public bool CanMakeTreeItem(LibClang.Cursor c)
        {
            switch(c.Kind)
            {
                case (LibClang.CursorKind.ClassDecl):
                case (LibClang.CursorKind.StructDecl):
                case (LibClang.CursorKind.UnionDecl):
                case (LibClang.CursorKind.EnumDecl):
                case (LibClang.CursorKind.Namespace):
                case (LibClang.CursorKind.Constructor):
                case (LibClang.CursorKind.Destructor):
                case (LibClang.CursorKind.VarDecl):
                case (LibClang.CursorKind.FieldDecl):
                case (LibClang.CursorKind.FunctionDecl):
                case (LibClang.CursorKind.CXXMethod):
                case (LibClang.CursorKind.FunctionTemplate):
                case (LibClang.CursorKind.ClassTemplate):
                case (LibClang.CursorKind.ClassTemplatePartialSpecialization):
                    return true;
            }

            return false;
        }        
    }
}

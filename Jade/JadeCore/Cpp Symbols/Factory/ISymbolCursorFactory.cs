using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.CppSymbols
{
    public interface ISymbolCursorFactory
    {

        ClassDeclarationSymbol CreateClassDefinition(LibClang.Cursor c);
        MethodDeclarationSymbol CreateMethodDefinition(LibClang.Cursor c);
        ConstructorDeclarationSymbol CreateConstructorDefinition(LibClang.Cursor c);
        NamespaceSymbol CreateNamespace(LibClang.Cursor c);
        /// <summary>
        /// Return the appropriate ISymbolCursor based on the Cursor's Kind or null
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        ISymbolCursor Create(LibClang.Cursor c);

        bool CanCreate(LibClang.Cursor c);
    }
}

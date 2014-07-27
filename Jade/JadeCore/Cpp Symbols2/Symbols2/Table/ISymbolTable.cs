using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.CppSymbols2
{    
    public delegate void SymbolEvent<TSymbol>(TSymbol symbol);
    
    public interface ISymbolSet<TSymbol> : IEnumerable<TSymbol> where TSymbol : class
    {
        event SymbolEvent<TSymbol> ItemAdded;
        event SymbolEvent<TSymbol> ItemChanged;
        event SymbolEvent<TSymbol> ItemRemoved;

        IEnumerable<TSymbol> Items { get; }
    }
    
    public interface INamespaceSet : ISymbolSet<NamespaceSymbol>
    {
        NamespaceSymbol Global { get; }
        NamespaceSymbol Anonymous { get; }
    }

    public interface IClassSet : ISymbolSet<ClassDeclarationSymbol>
    {        
    }

    public interface IMethodSet : ISymbolSet<MethodDeclarationSymbol>
    {
    }

    public interface ISymbolTable
    {
    //    INamespaceSet Namespaces { get; }



        /*
         * Enums
         * Functions
         * Unions
         * Variables
         * 
         * 
         * */

        void Update(LibClang.TranslationUnit tu);
    }

    /// <summary>
    /// Symbol Table for a set of Translation Units
    /// </summary>
}

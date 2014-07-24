using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.CppSymbols
{    
    public delegate void SymbolEvent<TSymbol>(TSymbol symbol);
 
   
    public interface ISymbolSet<TSymbol> : IEnumerable<TSymbol>
    {
        event SymbolEvent<TSymbol> ItemAdded;
        event SymbolEvent<TSymbol> ItemChanged;
        event SymbolEvent<TSymbol> ItemRemoved;
    }
    
    public interface asd
    {
        /// <summary>
        /// Class declarations
        /// </summary>
        ISymbolSet<ClassDeclarationSymbol> Classes { get; }

        ISymbolSet<NamespaceSymbol> Namespaces { get; }
    }

    public interface IFileSymbolTable
    {

    
    }

    /// <summary>
    /// Symbol Table for a set of Translation Units
    /// </summary>
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Symbols
{
    public interface ISymbol
    {
        //location
        //tracking?
        //Cursor
        LibClang.Cursor Cursor { get; }

        CppCodeBrowser.ICodeLocation Location { get; }
        
    }
}

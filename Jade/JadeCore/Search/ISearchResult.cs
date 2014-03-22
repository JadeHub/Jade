using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Search
{
    public interface ISearchResult
    {
        uint Rank { get; }
        string Summary { get; }

        CppCodeBrowser.ICodeLocation Location { get; }
        int Extent { get; }

        IFileHandle File { get; }
    }
}

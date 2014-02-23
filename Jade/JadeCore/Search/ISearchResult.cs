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
        FilePath Path { get; }
        int FileOffset { get; }
    }
}

using JadeUtils.IO;
using CppCodeBrowser;

namespace JadeCore.Search
{
    public class CodeSearchResult : ISearchResult
    {
        public CodeSearchResult(uint rank, ICodeLocation location, int extent)
        {
            Rank = rank;
            Summary = Summary;
            Location = location;
            Extent = extent;
        }

        public uint Rank { get; private set; }
        public string Summary { get; private set; }

        public CppCodeBrowser.ICodeLocation Location { get; private set; }
        public int Extent { get; private set; }

        public override string ToString()
        {
            return Location.Path.Str + "(" + Location.Offset + ":" + (Location.Offset + Extent) + ")";
        }
    }
}

using JadeUtils.IO;

namespace JadeCore.Search
{
    public class CodeSearchResult : ISearchResult
    {
        public CodeSearchResult(uint rank, FilePath path, int fileOffset)
        {
            Rank = rank;
            Summary = Summary;
            Path = path;
            FileOffset = fileOffset;
        }

        public uint Rank { get; private set; }
        public string Summary { get; private set; }
        public FilePath Path { get; private set; }
        public int FileOffset { get; private set; }

        public override string ToString()
        {
            return Path.Str + "(" + FileOffset + ")";
        }
    }
}

using JadeUtils.IO;

namespace JadeCore.Project
{
    public interface IFileItem : IItem
    {
        FilePath Path { get; }
        JadeUtils.IO.IFileHandle Handle { get; }
    }
}

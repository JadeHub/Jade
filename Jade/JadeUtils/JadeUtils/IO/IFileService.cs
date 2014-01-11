
namespace JadeUtils.IO
{
    public interface IFileService
    {
        event FileChangeEvent FileCreated;

        /// <summary>
        /// Create an IFileHandle representing a file at 'path'
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>New IFileHandle object</returns>
        IFileHandle MakeFileHandle(string path);

        /// <summary>
        /// Create an IFileHandle representing a file at 'path'
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>New IFileHandle object</returns>
        IFileHandle MakeFileHandle(FilePath path);        
    }
}

using JadeUtils.IO;

namespace CppCodeBrowser
{   
    public interface IProject
    {
        void AddSourceFile(FilePath path, string[] compilerArgs);
        IProjectIndex Index { get; }
    }

    public class Project : IProject
    {
        #region Data

        private readonly string _name;
        private readonly IIndexBuilder _indexer;

        #endregion

        public Project(string name, IIndexBuilder indexBuilder)
        {
            _name = name;
            _indexer = indexBuilder;
        }

        public void AddSourceFile(FilePath path, string[] compilerArgs)
        {
            _indexer.AddFile(path, compilerArgs);
        }

        public IProjectIndex Index
        {
            get { return _indexer.Index; }
        }
    }
}

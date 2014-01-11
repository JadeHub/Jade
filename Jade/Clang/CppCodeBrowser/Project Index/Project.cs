using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppCodeBrowser
{   
    public interface IProject
    {
        void AddSourceFile(string path, string[] compilerArgs);
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

        public void AddSourceFile(string path, string[] compilerArgs)
        {
            _indexer.IndexFile(path, compilerArgs);
        }

        public IProjectIndex Index
        {
            get { return _indexer.Index; }
        }
    }
}

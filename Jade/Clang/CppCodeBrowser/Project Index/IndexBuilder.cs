using System;

namespace CppCodeBrowser
{
    public interface IIndexBuilder : IDisposable
    {
        void IndexFile(string path, string[] compilerArgs);
        IProjectIndex Index { get; }
    }

    /// <summary>
    /// This is a simple synchronous implementation.
    /// </summary>
    public class IndexBuilder : IIndexBuilder
    {
        private readonly ProjectIndex _index;
        private readonly LibClang.Index _libClangIndex;

        public IndexBuilder()
        {
            _index = new ProjectIndex();
            _libClangIndex = new LibClang.Index(false, true);
        }

        public void Dispose()
        {
            //dispose translation units
        }

        public void IndexFile(string path, string[] compilerArgs)
        {
            LibClang.TranslationUnit tu = new LibClang.TranslationUnit(_libClangIndex, path);

            if (tu.Parse(compilerArgs) == false)
            {
                tu.Dispose();
                return;
            }
            _index.AddSourceFile(path, tu);
        }

        public IProjectIndex Index { get { return _index; } }
    }
}

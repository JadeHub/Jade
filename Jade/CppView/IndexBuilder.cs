using System;
using System.Diagnostics;

namespace CppView
{
    using JadeUtils.IO;

    public enum IndexBuilderState
    {
        Running,
        Suspended,
        Stopped
    }

    public enum IndexBuilderItemPriority
    {
        Immediate,
        High,
        Low
    }

    public interface IIndexBuilder
    {
        void Start();
        void Stop();

        IndexBuilderState State { get; }

        void AddSourceFile(JadeUtils.IO.IFileHandle file, IndexBuilderItemPriority priority);        
    }

    public class IndexBuilder : IIndexBuilder
    {
        private class Observer : LibClang.Indexer.Indexer.IObserver
        {
            private IndexBuilder _ib;
            private IProjectSourceIndex _sourceIndex;
            private LibClang.TranslationUnit _tu;

            internal Observer(IndexBuilder ib, IProjectSourceIndex sourceIndex, LibClang.TranslationUnit tu)
            {
                _ib = ib;
                _sourceIndex = sourceIndex;
                _tu = tu;
            }

            public bool Abort(LibClang.Indexer.Indexer indexer)
            {
                return false;
            }

            public void IncludeFile(LibClang.Indexer.Indexer indexer, string path, LibClang.SourceLocation[] includeStack)
            {
                IHeaderFile header = _sourceIndex.FileStore.FindOrAddHeaderFile(FilePath.Make(path));
                header.MentionedIn(_tu);
                //Add tu
                Debug.WriteLine("Include file " + path);
            }

            public void EntityDeclaration(LibClang.Indexer.Indexer indexer, LibClang.Indexer.DeclInfo decl)
            {
                FilePath path = JadeUtils.IO.FilePath.Make(decl.Location.File.Name);
                Debug.Assert(_sourceIndex.FileStore.ContainsFile(path));
                if (_sourceIndex.SymbolTable.HasDeclaration(decl.Cursor.Usr, path, decl.Location.Offset))
                {
                    return;
                }
                Declaration d = new Declaration(decl, path, _tu.GetCursorAt(decl.Location));
                _sourceIndex.SymbolTable.Add(d);
                //Debug.WriteLine("Decl " + d);
            }

            public void EntityReference(LibClang.Indexer.Indexer indexer, LibClang.Indexer.EntityReference reference)
            {
                FilePath path = JadeUtils.IO.FilePath.Make(reference.Location.File.Name);
                Debug.Assert(_sourceIndex.FileStore.ContainsFile(path));
                if (_sourceIndex.SymbolTable.HasReference(reference.ReferencedEntity.Usr, path, reference.Location.Offset) == false)
                {
                    IReference r = new Reference(reference.ReferencedEntity.Usr,
                                                 reference, path,
                                                 _sourceIndex.SymbolTable,
                                                 _tu.GetCursorAt(reference.Location));

                    _sourceIndex.SymbolTable.Add(r);
                    //   Debug.WriteLine(r);                    
                }
                else
                {
                    Debug.WriteLine("Dup");
                }
            }
        }

        #region Data

        private IndexBuilderState _state;
        
        private IProjectSourceIndex _sourceIndex;
        private LibClang.Index _index;
        private LibClang.Indexer.Indexer _indexer;
        private IntPtr _indexerSession;

        #endregion

        public IndexBuilder(IProjectSourceIndex sourceIndex)
        {
            _sourceIndex = sourceIndex;
            _state = IndexBuilderState.Stopped;
            _index = new LibClang.Index(true, true);
            _indexerSession = _index.CreateIndexingSession();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public IndexBuilderState State
        {
            get { return _state; }
        }

        public void AddSourceFile(JadeUtils.IO.IFileHandle file, IndexBuilderItemPriority priority)
        {
            ISourceFile f = _sourceIndex.FileStore.AddSourceFile(file.Path);
            Debug.Assert(f.TranslationUnit == null);

            bool reparse = false;

            if (f.TranslationUnit == null)
            {
                f.TranslationUnit = new LibClang.TranslationUnit(_index, file.Path.Str);
                f.TranslationUnit.Parse();
            }
            else
            {
                reparse = true;
            }

            if (reparse)
            {
                //reparse
            }
            else
            {
                _indexer = new LibClang.Indexer.Indexer(_index, f.TranslationUnit);
                _indexer.Parse(new Observer(this, _sourceIndex, f.TranslationUnit), _indexerSession);
            }
        }
    }
}

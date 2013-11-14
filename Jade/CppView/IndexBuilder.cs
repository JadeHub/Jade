using System;
using System.Diagnostics;

namespace CppView
{
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
            private IProjectSymbolTable _indexData;
            private IProjectSourceIndex _sourceIndex;

            internal Observer(IndexBuilder ib, IProjectSymbolTable indexData, IProjectSourceIndex sourceIndex)
            {
                _ib = ib;
                _indexData = indexData;
                _sourceIndex = sourceIndex;
            }

            public bool Abort(LibClang.Indexer.Indexer indexer)
            {
                return false;
            }

            public void PPIncludeFile(LibClang.Indexer.Indexer indexer, LibClang.Indexer.IncludeFileInfo includeFile)
            {
                Debug.WriteLine("Include file " + includeFile.File.Name);
            }

            public void EntityDeclaration(LibClang.Indexer.Indexer indexer, LibClang.Indexer.DeclInfo decl)
            {
                ISourceFile file = _sourceIndex.FindOrAdd(JadeUtils.IO.FilePath.Make(decl.Location.File.Name));
                if (_indexData.HasDeclaration(decl.Cursor.Usr, file, decl.Location.Offset))
                {
                    return;
                }
                Declaration d = new Declaration(decl, file);
                _indexData.Add(d);
                Debug.WriteLine("Decl " + d);
            }

            public void EntityReference(LibClang.Indexer.Indexer indexer, LibClang.Indexer.EntityReference reference)
            {
                ISourceFile file = _sourceIndex.FindOrAdd(JadeUtils.IO.FilePath.Make(reference.Location.File.Name));
                if (_indexData.HasReference(reference.ReferencedEntity.Usr, file, reference.Location.Offset) == false)
                {
                    IReference r = new Reference(reference.ReferencedEntity.Usr, reference, file, _indexData);
                    _indexData.Add(r);
                    Debug.WriteLine(r);
                }
            }
        }

        #region Data

        private IndexBuilderState _state;
        private IProjectSourceIndex _indexData;
        private IProjectSourceIndex _sourceIndex;

        private LibClang.Index _index;
        private LibClang.Indexer.Indexer _indexer;
        private IntPtr _indexerSession;

        #endregion

        public IndexBuilder(IProjectSourceIndex indexData, IProjectSourceIndex sourceIndex)
        {
            _indexData = indexData;
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
            _indexer = new LibClang.Indexer.Indexer(_index, file.Path.Str);
            _indexer.Parse(new Observer(this, _indexData.SymbolTable, _sourceIndex), _indexerSession);
        }
    }
}

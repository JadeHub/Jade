using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using JadeUtils.IO;
using LibClang;

namespace JadeCore.Symbols
{
    public enum TableUpdateType
    {
        Editing,
        Complete
    }

    public interface ITable
    {
        IEnumerable<ISymbolDeclaration> Declarations { get; }
        IEnumerable<ISymbolReference> References { get; }
    }
        
    public interface ITableUpdate
    {
        void Update(IDictionary<FilePath, IList<LibClang.Indexer.DeclInfo>> decls,
                    IDictionary<FilePath, IList<LibClang.Indexer.EntityReference>> refs);

        bool FilterFilePath(FilePath path);
    }

    /// <summary>
    /// Symbol table for a Source Project
    /// </summary>
    public partial class ProjectTable : ITable, ITableUpdate
    {        
        private Project.IProject _project;
        private TableBuilder _builder;
        private Dictionary<FilePath, ProjectFileTable> _fileTables;
        
        
        public ProjectTable(Project.IProject project)
        {
            _project = project;
            _builder = new TableBuilder(_project, this);
        }

        #region ITable implementation

        public IEnumerable<ISymbolDeclaration> Declarations 
        {
            get 
            {
                
                return null; 
            }
        }

        public IEnumerable<ISymbolReference> References
        {
            get { return null; }
        }

        #endregion

        #region ITableUpdate implementation

        private ProjectFileTable FileOrAddFileTable(FilePath path)
        {
            ProjectFileTable result = null;

            if(_fileTables.TryGetValue(path, out result) == false)
            {
                result = new ProjectFileTable(path);
                _fileTables.Add(path, result);
            }
            return result;
        }

        public void Update(IDictionary<FilePath, IList<LibClang.Indexer.DeclInfo>> decls,
                            IDictionary<FilePath, IList<LibClang.Indexer.EntityReference>> refs)
        {
            _fileTables = new Dictionary<FilePath, ProjectFileTable>();

            foreach(FilePath path in decls.Keys)
            {
                ProjectFileTable fileTable = FileOrAddFileTable(path);
                fileTable.Update(decls);                
            }
        }

        public bool FilterFilePath(FilePath path)
        {
            return _project.FindFileItem(path) != null;
        }

        #endregion
    }
    
}

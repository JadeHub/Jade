using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;
using LibClang;

namespace JadeCore.Symbols
{
    public partial class ProjectTable : ITable, ITableUpdate
    {
        /// <summary>
        /// Symbol table for a file (source or header)
        /// </summary>
        private class ProjectFileTable : ITable
        {
            private FilePath _path;

            internal ProjectFileTable(FilePath path)
            {
                _path = path;
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

            internal void Update(IDictionary<FilePath, IList<LibClang.Indexer.DeclInfo>> decls)
            { }

            internal bool FilterFilePath(FilePath path)
            {
                return path == _path;
            }
        }
    }
}

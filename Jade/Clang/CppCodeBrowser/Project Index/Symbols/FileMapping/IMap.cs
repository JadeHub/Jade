using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace CppCodeBrowser.Symbols.FileMapping
{
    public interface IProjectFileMaps
    {
        void UpdateDeclarationMapping(FilePath path, int startOffset, int endOffset, ISymbol symbol);
        IFileMap GetMap(FilePath path);
    }

    public class ProjectFileMaps : IProjectFileMaps
    {
        private Dictionary<FilePath, IFileMap> _fileMappings;

        public ProjectFileMaps()
        {
            _fileMappings = new Dictionary<FilePath, IFileMap>();
        }

        public void UpdateDeclarationMapping(FilePath path, int startOffset, int endOffset, ISymbol symbol)
        {
            IFileMap map = GetFileMap(path);
            if (map != null)
                map.AddMapping(startOffset, endOffset, symbol);
        }

        public IFileMap GetMap(FilePath path)
        {
            IFileMap result = null;
            _fileMappings.TryGetValue(path, out result);
            return result;
        }

        private IFileMap GetFileMap(FilePath path)
        {
            IFileMap result = null;
            if(_fileMappings.TryGetValue(path, out result) == false)
            {
                result = new FileMap(path);
                _fileMappings.Add(path, result);
            }
            return result;
        }
    }
}

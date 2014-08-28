using JadeUtils.IO;
using LibClang;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace CppCodeBrowser
{   
    public delegate void ProjectItemEvent(FilePath path);

    public interface IProjectIndex
    {
        TranslationUnit FindTranslationUnit(FilePath path);        
        LibClang.Index LibClangIndex { get; }
        void UpdateParseResult(ParseResult result);

        IEnumerable<ParseResult> ParseResults { get;}
        Symbols.ISymbolTable Symbols { get; }
        Symbols.FileMapping.IProjectFileMaps FileSymbolMaps { get; }
    }

    public class ProjectIndex : IProjectIndex
    {
        private readonly Dictionary<FilePath, ParseResult> _parseResults;

        private object _lock;
        private readonly LibClang.Index _libClangIndex;

        private Symbols.ProjectSymbolTable _symbols;
        private Symbols.FileMapping.IProjectFileMaps _fileSymbolMappings;
                        
        public ProjectIndex()
        {
            _lock = new object();
            _libClangIndex = new LibClang.Index(false, true);
            _parseResults = new Dictionary<FilePath, ParseResult>();
            _fileSymbolMappings = new Symbols.FileMapping.ProjectFileMaps();
            _symbols = new Symbols.ProjectSymbolTable(_fileSymbolMappings);
        }

        public TranslationUnit FindTranslationUnit(FilePath path)
        {
            lock (_lock)
            {
                if (_parseResults.ContainsKey(path))
                    return _parseResults[path].TranslationUnit;
            }
            return null;
        }

        public void UpdateParseResult(ParseResult result)
        {
            lock (_lock)
            {
                if(_parseResults.ContainsKey(result.Path))
                {
                    _parseResults[result.Path] = result;
                }
                else
                {
                    _parseResults.Add(result.Path, result);
                }
            }
            return;
        }
        
        public LibClang.Index LibClangIndex { get { return _libClangIndex; } }

        public Symbols.FileMapping.IProjectFileMaps FileSymbolMaps { get { return _fileSymbolMappings; } }
        public Symbols.ISymbolTable Symbols { get { return _symbols; } }
        public IEnumerable<ParseResult> ParseResults { get { return _parseResults.Values; } }
    }
}

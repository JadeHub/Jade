using JadeUtils.IO;
using LibClang;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CppCodeBrowser
{
    public class ParseFile
    {
        public ParseFile(FilePath path, object data, string content)
        {
            Path = path;
            Data = data;
            Content = content;
        }

        public FilePath Path { get; private set; }
        public object Data { get; private set; }
        public string Content { get; private set; }
    }

    public class ParseResult : IDisposable
    {
        private FilePath _path;
        private LibClang.TranslationUnit _translationUnit;
        private List<ParseFile> _files;

        public ParseResult(FilePath path, IUnsavedFileProvider unsavedFiles, LibClang.TranslationUnit tu)
        {
            _path = path;
            _translationUnit = tu;
            _files = new List<ParseFile>(unsavedFiles.UnsavedFiles);
        }

        public void Dispose()
        {
            if (_translationUnit != null)
            {
                _translationUnit.Dispose();
                _translationUnit = null;
            }
        }

        public LibClang.TranslationUnit TranslationUnit { get { return _translationUnit; } }
        public IEnumerable<ParseFile> Files { get { return _files; } }
    }

    public static class Parser
    {
        public static ParseResult Parse(ProjectIndex index, FilePath path, string[] compilerArgs, IUnsavedFileProvider unsavedFiles)
        {
            TranslationUnit tu = new TranslationUnit(index.LibClangIndex, path.Str);
                                    
            List<Tuple<string, string>> unsavedList = new List<Tuple<string, string>>();
            foreach(var i in unsavedFiles.UnsavedFiles)
            {
                unsavedList.Add(new Tuple<string, string>(i.Path.Str, i.Content));
            }
            if (tu.Parse(compilerArgs, unsavedList) == false)
            {
                tu.Dispose();
                return null;
            }

            return new ParseResult(path, unsavedFiles, tu);;
        }
    }
    
    public interface IUnsavedFileProvider
    {
       /// <summary>
       /// Return a list of Tuples containing path, content
       /// </summary>
       /// <returns></returns>
        IList<Tuple<string, string>> GetUnsavedFiles();
        IEnumerable<ParseFile> UnsavedFiles { get; }
    }
    
    public class ProjectIndexBuilder : IIndexBuilder
    {
        private bool _disposed = false;
        private readonly ProjectIndex _index;

        //Set of all parsed Tus for Disposal
        private readonly HashSet<TranslationUnit> _allTus;

        private TaskScheduler _callbackScheduler;
        private IUnsavedFileProvider _unsavedFilesProvider;
        private object _lock = new object();
                
        public ProjectIndexBuilder(TaskScheduler callbackSCheduler, IUnsavedFileProvider unsavedFiles)
        {
            _callbackScheduler = callbackSCheduler;
            _unsavedFilesProvider = unsavedFiles;
            _index = new ProjectIndex();
            _allTus = new HashSet<TranslationUnit>();
        }

        public void Dispose()
        {
            if(_disposed) return;

            lock (_lock)
            {
                foreach (TranslationUnit tu in _allTus)
                {
                    tu.Dispose();
                }
                _allTus.Clear();
                _disposed = true;
            }
        }

        public bool ParseFile(FilePath path, string[] compilerArgs)
        {            
            if (path.FileName == "main.cpp" && done)
            {

                return true; ;
            }
            if (path.FileName == "main.cpp")
                done = true;


            if (_disposed) return false;

            //lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine("**Parsing " + path.FileName);

                ParseResult result = Parser.Parse(_index, path, compilerArgs, _unsavedFilesProvider);
                if (result != null)
                {
                    _index.UpdateSourceFile(path, result.TranslationUnit);

                    //Perform on gui thread
                    Task.Factory.StartNew(() =>
                        {
                           // lock (_lock)
                            {
                                System.Diagnostics.Debug.WriteLine("**Indexing " + path.FileName);
                                IndexTranslationUnit(result.TranslationUnit);
                                _index.RaiseItemUpdatedEvent(path);
                            }
                        }, CancellationToken.None, TaskCreationOptions.None, _callbackScheduler);
                }
            }
            return true;
        }

        public IProjectIndex Index 
        { 
            get
            {
                if (_disposed) return null;
                return _index; 
            }
        }

        static bool done = false;

        private void IndexTranslationUnit(TranslationUnit tu)        
        {           
            
            foreach (Cursor c in tu.Cursor.Children)
            {
                IndexCursor(c);
            }
        }
        
        private bool IsIndexCursorKind(CursorKind k)
        {
          //  return k != CursorKind.MacroDefinition && k!= CursorKind.MacroExpansion && k != CursorKind.MacroInstantiation && k != CursorKind.UnexposedDecl;
            return 
                (
                    CursorKinds.IsClassStructEtc(k) ||
                    k == LibClang.CursorKind.CXXMethod ||
                    k == LibClang.CursorKind.Constructor ||
                    k == LibClang.CursorKind.Destructor ||
                    k == LibClang.CursorKind.FieldDecl ||
                    k == LibClang.CursorKind.ClassTemplate ||
                    k == LibClang.CursorKind.Namespace ||
                    k == LibClang.CursorKind.FunctionDecl ||
                    k == LibClang.CursorKind.VarDecl ||
                    k == LibClang.CursorKind.EnumDecl ||
                    k == LibClang.CursorKind.EnumConstantDecl ||
                    k == CursorKind.ConversionFunction ||
                    k == CursorKind.ParmDecl ||
                    k == CursorKind.TemplateTypeParameter ||
                    CursorKinds.IsReference(k) ||
                    CursorKinds.IsStatement(k)/* ||
                    CursorKinds.IsExpression(k)*/
                );
        }

        private void IndexDefinitionCursor(Cursor c)
        {
            _index.Symbols.UpdateDefinition(c);
        }

        private void IndexReferenceCursor(Cursor c)
        {
            _index.Symbols.UpdateReference(c);
        }

        private bool FilterCursor(Cursor c)
        {
            if (IsIndexCursorKind(c.Kind) == false) return false;
            if (c.Location == null || c.Location.File == null) return false;

            return true;
        }

        private void IndexCursor(Cursor c)
        {
            if (FilterCursor(c) == false) return;
            
            if (CursorKinds.IsDefinition(c.Kind))
            {
                IndexDefinitionCursor(c);
            }
            else if (c.CursorReferenced != null)
            {
                if (FilterCursor(c.CursorReferenced))
                    IndexReferenceCursor(c);
            } 

            foreach (Cursor child in c.Children)
            {
                IndexCursor(child);
            }
        }
    }
}

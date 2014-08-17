using JadeUtils.IO;
using LibClang;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CppCodeBrowser
{   
    public interface IUnsavedFileProvider
    {
       /// <summary>
       /// Return a list of Tuples containing path, content
       /// </summary>
       /// <returns></returns>
        IList<Tuple<string, string>> GetUnsavedFiles();
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
        private Func<FilePath, bool> _fileFilter;
        
        public ProjectIndexBuilder(Func<FilePath, bool> fileFilter, TaskScheduler callbackSCheduler, IUnsavedFileProvider unsavedFiles)
        {
            _callbackScheduler = callbackSCheduler;
            _unsavedFilesProvider = unsavedFiles;
            _fileFilter = fileFilter;
            _index = new ProjectIndex(fileFilter);
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
            if (_disposed) return false;

            lock (_lock)
            {
                LibClang.TranslationUnit tu = new LibClang.TranslationUnit(_index.LibClangIndex, path.Str);

                //pass in unsaved files
                if (tu.Parse(compilerArgs, _unsavedFilesProvider.GetUnsavedFiles()) == false)
                {
                    tu.Dispose();
                    return false;
                }
                  
                //Perform on gui thread
                Task.Factory.StartNew(() => 
                    {
                        lock (_lock)
                        {
                            _index.UpdateSourceFile(path, tu);
                            IndexTranslationUnit(tu);
                            _index.RaiseItemUpdatedEvent(path);
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, _callbackScheduler);
                
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
            FilePath p = FilePath.Make(tu.File.Name);
            if (p.FileName == "main.cpp" && done)
            {
                
                return;
            }
            if(p.FileName == "main.cpp")
               done = true;

            System.Diagnostics.Debug.WriteLine("**Indexing " + p.FileName);

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

          //  FilePath path = FilePath.Make(c.Location.File.Name);
       //     if (_fileFilter(path) == false) return false;

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

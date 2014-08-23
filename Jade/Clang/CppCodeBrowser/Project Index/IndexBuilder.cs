﻿using JadeUtils.IO;
using LibClang;
using System;
using System.Linq;
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

        public ParseResult ParseFile(FilePath path, string[] compilerArgs)
        {   
            if (_disposed) return null;

          //  lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine("**Parsing " + path.FileName);

                return Parser.Parse(_index, path, compilerArgs, _unsavedFilesProvider);
                /*
                if (result != null)
                {
                    _index.UpdateSourceFile(path, result.TranslationUnit);

                    //Perform on gui thread
                    Task.Factory.StartNew(() =>
                        {
                        //    lock (_lock)
                            {
                                System.Diagnostics.Debug.WriteLine("**Indexing " + path.FileName);
                                IndexTranslationUnit(result.TranslationUnit);
                                _index.RaiseItemUpdatedEvent(path);
                            }
                        }, CancellationToken.None, TaskCreationOptions.None, _callbackScheduler);
                }
                return result;*/
            }
            
        }

        public void IndexTranslationUnit(ParseResult parseResult)
        {
            _index.UpdateSourceFile(parseResult.Path, parseResult.TranslationUnit);

            //Perform on gui thread
            Task.Factory.StartNew(() =>
            {
                //    lock (_lock)
                {
                    System.Diagnostics.Debug.WriteLine("**Indexing " + parseResult.Path.FileName + " version:" + parseResult.GetFileVersion(parseResult.Path));
                    IndexTranslationUnit(parseResult.TranslationUnit);
                    _index.RaiseItemUpdatedEvent(parseResult.Path);
                }
            }, CancellationToken.None, TaskCreationOptions.None, _callbackScheduler);
        }

        public IProjectIndex Index 
        { 
            get
            {
                if (_disposed) return null;
                return _index; 
            }
        }

     //   static bool done = false;

        private void IndexTranslationUnit(TranslationUnit tu)        
        {           
            
            foreach (Cursor c in tu.Cursor.Children)
            {
                IndexCursor(c);
            }
        }
        
        private bool IsIndexCursorKind(CursorKind k)
        {
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
                    k == LibClang.CursorKind.ParamDecl ||
                    k == CursorKind.ConversionFunction ||
                    k == CursorKind.ParamDecl ||
                    k == CursorKind.TemplateTypeParameter ||
                    k == CursorKind.CallExpr ||
                    k == CursorKind.DeclRefExpr ||
                    k == CursorKind.MemberRefExpr ||
                    CursorKinds.IsReference(k) ||
                    CursorKinds.IsStatement(k) ||
                    CursorKinds.IsExpression(k)
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

            if (c.Spelling == "staticInt")
            {
                int i = 0;
            }

            if (CursorKinds.IsDefinition(c.Kind))
            {
                IndexDefinitionCursor(c);
            }
            else if (c.CursorReferenced != null)// && c.Kind != CursorKind.CallExpr)
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

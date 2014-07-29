using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using JadeUtils.IO;
using LibClang;

namespace JadeCore.Symbols
{
    public class TableBuilder
    {
        private class Observer : LibClang.Indexer.Indexer.IObserver
        {
            private Action<LibClang.Indexer.DeclInfo> _onDecl;
            private Action<LibClang.Indexer.EntityReference> _onRef;

            public Observer(Action<LibClang.Indexer.DeclInfo> onDecl, Action<LibClang.Indexer.EntityReference> onRef)
            {
                _onDecl = onDecl;
                _onRef = onRef;
            }

            public bool Abort(LibClang.Indexer.Indexer indexer) { return false; }
            public void IncludeFile(LibClang.Indexer.Indexer indexer, string path, SourceLocation[] inclutionStack) { }

            public void EntityDeclaration(LibClang.Indexer.Indexer indexer, LibClang.Indexer.DeclInfo decl)
            {
                _onDecl(decl);

                //Debug.WriteLine("Decl: " + decl.EntityInfo.Name + " " + decl.EntityInfo.Kind + " " + decl.Location + " " + decl.EntityInfo.Usr);
            }

            public void EntityReference(LibClang.Indexer.Indexer indexer, LibClang.Indexer.EntityReference reference)
            {
                _onRef(reference);
            }
        }

        private Project.IProject _project;
        private IntPtr _indexSession;
        private ITableUpdate _tableUpdate;

        public TableBuilder(Project.IProject project, ITableUpdate tableUpdate)
        {
            _project = project;
            _tableUpdate = tableUpdate;
            //todo dispose
            _indexSession = project.Index.LibClangIndex.CreateIndexingSession();

            project.Index.ItemUpdated += Index_ItemUpdated;
        }

        private void Index_ItemUpdated(FilePath path)
        {
        
            /*if (_project.FindFileItem(path) == null) return;

            CppCodeBrowser.IProjectFile fileIndex = _project.Index.FindProjectItem(path);

            if (fileIndex != null && fileIndex is CppCodeBrowser.ISourceFile)
            {
                Update((fileIndex as CppCodeBrowser.ISourceFile).TranslationUnit);
            }*/
        }

        public Project.IProject Project { get { return _project; } }

        public void Update(TranslationUnit tu)
        {
            FilePath t = FilePath.Make(tu.File.Name);

            if (t.FileName != "main.cpp") return;

            IDictionary<FilePath, IList<LibClang.Indexer.DeclInfo>> decls = new Dictionary<FilePath, IList<LibClang.Indexer.DeclInfo>>();
            IDictionary<FilePath, IList<LibClang.Indexer.EntityReference>> refs = new Dictionary<FilePath, IList<LibClang.Indexer.EntityReference>>();
            HashSet<LibClang.SourceLocation> refLocations = new HashSet<SourceLocation>();
            
            LibClang.Indexer.Indexer.Parse(tu, 
                                        new Observer(
                                            delegate(LibClang.Indexer.DeclInfo decl)
                                            {
                                                FilePath path = FilePath.Make(decl.FileName);
                                                if(_tableUpdate.FilterFilePath(path))
                                                {
                                                    IList<LibClang.Indexer.DeclInfo> fileDecls = null;
                                                    if(decls.TryGetValue(path, out fileDecls) == false)
                                                    {
                                                        fileDecls = new List<LibClang.Indexer.DeclInfo>();
                                                        decls.Add(path, fileDecls);
                                                    }
                                                    LibClang.SourceLocation loc = decl.Location;
                                                    fileDecls.Add(decl);                                                    
                                                }
                                            },
                                            delegate(LibClang.Indexer.EntityReference reference)
                                            {
                                                FilePath path = FilePath.Make(reference.FileName);
                                                if(_tableUpdate.FilterFilePath(path) && refLocations.Add(reference.Location))
                                                {
                                                    IList<LibClang.Indexer.EntityReference> fileRefs = null;
                                                    if(refs.TryGetValue(path, out fileRefs) == false)
                                                    {
                                                        fileRefs = new List<LibClang.Indexer.EntityReference>();
                                                        refs.Add(path, fileRefs);
                                                    }
                                                    fileRefs.Add(reference);
                                                }
                                            }
                                            ),                                        
                                        _indexSession);
            _tableUpdate.Update(decls, refs);
            
            foreach (FilePath path in decls.Keys)
            {
                var fileDecls = decls[path];
                Debug.WriteLine("Updating " + path.FileName);
                var list = from d in fileDecls orderby d.Location select d;
                foreach (var d in list)
                {
                    Debug.Assert(path == FilePath.Make(d.Location.File.Name));
                    //FilePath path = FilePath.Make(d.Location.File.Name);
                    Cursor c = d.Cursor;
                    FilePath entityPath = FilePath.Make(d.EntityInfo.Cursor.Location.File.Name);

                    Debug.Write(string.Format("{0,-20} {1,-20} {2} {3}:{4} {5,-5} {6,-5}",
                        d.EntityInfo.Kind, d.EntityInfo.Name, path.FileName, d.Location.Line, d.Location.Column,
                        d.IsDefinition ? "True" : "False", d.IsRedefinition ? "True" : "False"));

                    if (d.Location != d.EntityInfo.Cursor.Location)
                    {
                        Debug.Write(string.Format(" {0} {1}:{2}",
                                    entityPath.FileName, d.EntityInfo.Cursor.Location.Line, d.EntityInfo.Cursor.Location.Column));
                    }
                    Debug.Write("\r");
                }
                /*
                Debug.WriteLine("References...");
                foreach(var r in refs)
                {
                    FilePath path = FilePath.Make(r.Location.File.Name);

                            Debug.Write(string.Format("{0,-20} {1,-20} {2} {3}:{4}",
                            r.ReferencedEntity.Kind, r.ReferencedEntity.Name, path.FileName, r.Location.Line, r.Location.Column
                            ));
                            Debug.Write("\r");
                
                
                }*/
            }
        }

        void Write(string s, int len)
        {
            Debug.Write(s);
            for(int i =0; i < (len-s.Length);i++)
            {
                Debug.Write(' ');
            }
        }
    }
}

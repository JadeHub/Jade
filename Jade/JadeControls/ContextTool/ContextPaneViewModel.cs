using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using JadeUtils.IO;
using CppCodeBrowser.Symbols;

namespace JadeControls.ContextTool
{
    public class ContextPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.IEditorController _editorController;
        private ObservableCollection<DeclarationViewModel> _root;
        private HashSet<FilePath> _files;
        
        public ContextPaneViewModel(JadeCore.IEditorController editCtrl)
        {
            Title = "Context Tool";
            ContentId = "ContextToolPane";
            _editorController = editCtrl;
            _root = new ObservableCollection<DeclarationViewModel>();
            _files = new HashSet<FilePath>();

            JadeCore.Services.Provider.CppParser.TranslationUnitIndexed += OnCppParserTranslationUnitIndexed;
        }

        private void OnCppParserTranslationUnitIndexed(CppCodeBrowser.ParseResult result)
        {
            UpdateTree(result.Index);
        }

        private DeclarationViewModel FindRootLevelItem(string usr)
        {
            foreach(DeclarationViewModel d in _root)
            {
                if (d.Usr == usr)
                    return d;
            }
            return null;
        }

        private DeclarationViewModel FindOrAddNamespaceNode(NamespaceDecl ns)
        {
            List<NamespaceDecl> parents = new List<NamespaceDecl>();
            NamespaceDecl temp = ns.ParentNamespace;
            while (temp != null)
            {
                parents.Insert(0, temp);
                temp = temp.ParentNamespace;
            }

            DeclarationViewModel parentNode = null;

            if (parents.Count > 0)
            {
                foreach (NamespaceDecl parent in parents)
                {
                    if (parentNode == null)
                    {
                        parentNode = FindRootLevelItem(parent.Usr);
                    }
                    else
                    {
                        parentNode = parentNode.FindOrAddChildDecl(parent);
                        if (parentNode == null)
                            return null;
                    }
                }
                return parentNode.FindOrAddChildDecl(ns);
            }
            DeclarationViewModel result = FindRootLevelItem(ns.Usr);
            if(result == null)
            {
                result = new DeclarationViewModel(parentNode, ns);
                _root.Add(result);
            }
            return result;
        }

        private DeclarationViewModel FindOrAddParentNode(IDeclaration decl)
        {            
            if(decl is NamespaceDecl)
                return FindOrAddNamespaceNode(decl as NamespaceDecl);
            if(decl is ClassDecl)
                return FindOrAddClassNode(decl as ClassDecl);
            return null;
        }

        private DeclarationViewModel FindOrAddClassNode(ClassDecl c)
        {
            if(c.Parent == null)
            {
                DeclarationViewModel vm = FindRootLevelItem(c.Usr);
                if (vm != null)
                    return vm;

                vm = new DeclarationViewModel(null, c);
                _root.Add(vm);
                return vm;
            }
            Debug.Assert(c.Parent != null);
            var parentnode = FindOrAddParentNode(c.Parent);
            return parentnode.FindOrAddChildDecl(c);
        }

        private DeclarationViewModel FindOrAddFunctionNode(FunctionDecl f)
        {
            if (f.Parent == null)
            {
                DeclarationViewModel vm = FindRootLevelItem(f.Usr);
                if (vm != null)
                    return vm;

                vm = new DeclarationViewModel(null, f);
                _root.Add(vm);
                return vm;
            }

            Debug.Assert(f.Parent != null);
            var parentnode = FindOrAddParentNode(f.Parent);
            return parentnode.FindOrAddChildDecl(f);
        }

        private DeclarationViewModel FindOrAddEnumNode(EnumDecl e)
        {
            Debug.Assert(e.Parent != null);
            var parentnode = FindOrAddParentNode(e.Parent);
            return parentnode.FindOrAddChildDecl(e);
        }

        private DeclarationViewModel FindOrAddEnumConstantNode(EnumConstantDecl c)
        {
            DeclarationViewModel enumNode = FindOrAddEnumNode(c.Parent);
            return enumNode.FindOrAddChildDecl(c);
        }

        private bool FilterDeclaration(IDeclaration decl, ISet<FilePath> projectFiles)
        {
            _files.Add(decl.Location.Path);

            return true;
            //if (JadeCore.Services.Provider.WorkspaceController.CurrentWorkspace != null)
               // return JadeCore.Services.Provider.WorkspaceController.CurrentWorkspace.ContainsFile(decl.Location.Path);
            //return false;
            //return projectFiles.Contains(decl.Location.Path);
        }
                
        private void UpdateTree(CppCodeBrowser.IProjectIndex index)
        {
            ISymbolTable symbols = index.Symbols;
            ISet<FilePath> files = JadeCore.Services.Provider.WorkspaceController.CurrentWorkspace.Files;

            foreach(NamespaceDecl ns in symbols.Namespaces)
            {
                if (FilterDeclaration(ns, files))
                    FindOrAddNamespaceNode(ns);
            }

            foreach(ClassDecl c in symbols.Classes)
            {
                if (FilterDeclaration(c, files))
                    FindOrAddClassNode(c);
            }

            foreach(MethodDecl m in symbols.Methods)
            {
                if (FilterDeclaration(m, files))
                {
                    DeclarationViewModel parentClass = FindOrAddClassNode(m.Class);
                    parentClass.FindOrAddChildDecl(m);
                }
            }

            foreach(FieldDecl f in symbols.Fields)
            {
                if (FilterDeclaration(f, files))
                {
                    DeclarationViewModel parentClass = FindOrAddClassNode(f.Class);
                    parentClass.FindOrAddChildDecl(f);
                }
            }

            foreach(ConstructorDecl c in symbols.Constructors)
            {
                if (FilterDeclaration(c, files))
                {
                    if (c.Class != null)
                    {
                        DeclarationViewModel parentClass = FindOrAddClassNode(c.Class);
                        parentClass.FindOrAddChildDecl(c);
                    }
                }
            }

            foreach (DestructorDecl d in symbols.Destructors)
            {
                if (FilterDeclaration(d, files))
                {
                    if (d.Class != null)
                    {
                        DeclarationViewModel parentClass = FindOrAddClassNode(d.Class);
                        parentClass.FindOrAddChildDecl(d);
                    }
                }
            }

            foreach(EnumDecl e in symbols.Enums)
            {
                if (FilterDeclaration(e, files))
                    FindOrAddEnumNode(e);
            }

            foreach (EnumConstantDecl c in symbols.EnumConstants)
            {
                if (FilterDeclaration(c, files))
                    FindOrAddEnumConstantNode(c);
            }

            foreach(FunctionDecl f in symbols.Functions)
            {
                if (FilterDeclaration(f, files))
                    FindOrAddFunctionNode(f);
            }
            OnPropertyChanged("RootItems");
            OnPropertyChanged("FileNames");
        }

        public ObservableCollection<DeclarationViewModel> RootItems { get { return _root; } }

        public void BrowseToSelectedSymbol()
        {
            DeclarationViewModel selected = FindSelectedDeclaration();
            if(selected != null)
            {
                selected.BrowseToLocation();
            }
        }

        private DeclarationViewModel FindSelectedDeclaration()
        {
            foreach(DeclarationViewModel rootItem in _root)
            {
                DeclarationViewModel selected = rootItem.FindSelectedDeclaration();
                if (selected != null)
                    return selected;
            }
            return null;
        }
                
        public IEnumerable<string> FileNames
        {
            get
            {
                return from FilePath path in _files select path.FileName;
            }
        }

        //public FileViewModel CurrentFile { get; private set; }
        /*
        private void SelectPath(string path)
        {
            string [] parts = path.Split('/');

            if (parts.Length == 0) return;

            if (parts[0] != CurrentFile.Name) return;

            ITreeItem item = CurrentFile;
            for(int i = 1; i < parts.Length && item != null ; i++)
            {
                item.Expanded = true;
                var child = item.FindChild(parts[i]);
                if (child == null)
                    break;
                item = child;
                item.Selected = true;                
            }
        */
    }
}

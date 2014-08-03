using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;
using CppCodeBrowser.Symbols;

namespace JadeControls.ContextTool
{
    public class ContextPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.IEditorController _editorController;
        private CppCodeBrowser.IProjectIndex _currentIndex;
        private ObservableCollection<DeclarationViewModel> _root;
        
        public ContextPaneViewModel(JadeCore.IEditorController editCtrl)
        {
            Title = "Context Tool";
            ContentId = "ContextToolPane";
            _editorController = editCtrl;
            _editorController.ActiveDocumentChanged += EditorControllerActiveDocumentChanged;
            _root = new ObservableCollection<DeclarationViewModel>();            
        }

        private void EditorControllerActiveDocumentChanged(JadeCore.IEditorDoc newValue, JadeCore.IEditorDoc oldValue)
        {
            if (newValue == null)
            {
                if (_currentIndex != null)
                {
                    _currentIndex.ItemUpdated -= CurrentIndexItemUpdated;
                    _currentIndex = null;
                }
                return;
            }

            JadeCore.Project.IProject proj = newValue.Project;
            if (proj == null || proj.Index == null) return;

            if(_currentIndex != null)
            {
                _currentIndex.ItemUpdated -= CurrentIndexItemUpdated;
            }
            _currentIndex = proj.Index;
            _currentIndex.ItemUpdated += CurrentIndexItemUpdated;            
        }

        private void CurrentIndexItemUpdated(FilePath path)
        {
            if (_editorController.ActiveDocument == null) return;

          //  if(_editorController.ActiveDocument.File.Path == path)
            {
                UpdateTree();
            }
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
                
        private void UpdateTree()
        {
            ISymbolTable symbols = _currentIndex.Symbols;

            foreach(NamespaceDecl ns in symbols.Namespaces)
            {
                FindOrAddNamespaceNode(ns);
            }

            foreach(ClassDecl c in symbols.Classes)
            {
                FindOrAddClassNode(c);
            }

            foreach(MethodDecl m in symbols.Methods)
            {
                DeclarationViewModel parentClass = FindOrAddClassNode(m.Class);
                parentClass.FindOrAddChildDecl(m);
            }

            foreach(FieldDecl f in symbols.Fields)
            {
                DeclarationViewModel parentClass = FindOrAddClassNode(f.Class);
                parentClass.FindOrAddChildDecl(f);
            }

            foreach(ConstructorDecl c in symbols.Constructors)
            {
                if (c.Class != null)
                {
                    DeclarationViewModel parentClass = FindOrAddClassNode(c.Class);
                    parentClass.FindOrAddChildDecl(c);
                }
            }

            foreach (DestructorDecl d in symbols.Destructors)
            {
                if (d.Class != null)
                {
                    DeclarationViewModel parentClass = FindOrAddClassNode(d.Class);
                    parentClass.FindOrAddChildDecl(d);
                }
            }

            foreach(EnumDecl e in symbols.Enums)
            {
                FindOrAddEnumNode(e);
            }

            foreach (EnumConstantDecl c in symbols.EnumConstants)
            {
                FindOrAddEnumConstantNode(c);
            }

            foreach(FunctionDecl f in symbols.Functions)
            {
                FindOrAddFunctionNode(f);
            }
            OnPropertyChanged("RootItems");
        }

        public ObservableCollection<DeclarationViewModel> RootItems { get { return _root; } }

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

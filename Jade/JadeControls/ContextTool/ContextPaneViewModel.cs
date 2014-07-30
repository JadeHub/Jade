using System;
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

        public ContextPaneViewModel(JadeCore.IEditorController editCtrl)
        {
            Title = "Context Tool";
            ContentId = "ContextToolPane";
            _editorController = editCtrl;
            _editorController.ActiveDocumentChanged += EditorControllerActiveDocumentChanged;
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

            if(_editorController.ActiveDocument.File.Path == path)
            {
                UpdateTree(_editorController.ActiveDocument.File.Path);
            }
        }

        private ObservableCollection<DeclarationViewModel> _root = new ObservableCollection<DeclarationViewModel>();

        private DeclarationViewModel FindRootLevelNamespace(string usr)
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
                        parentNode = FindRootLevelNamespace(parent.Usr);
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
            DeclarationViewModel result = FindRootLevelNamespace(ns.Usr);
            if(result == null)
            {
                result = new DeclarationViewModel(parentNode, ns);
                _root.Add(result);
            }
            return result;
        }

        private DeclarationViewModel FindOrAddClassNode(ClassDecl c)
        {
            DeclarationViewModel classNode = null;
            if (c.Parent != null && c.Parent is NamespaceDecl)
            {
                var namespaceNode = FindOrAddNamespaceNode(c.Parent as NamespaceDecl);
                classNode = namespaceNode.FindOrAddChildDecl(c);                
            }
            return classNode;
        }
                
        private void UpdateTree(FilePath path)
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

            OnPropertyChanged("RootItems");
        }

        public ObservableCollection<DeclarationViewModel> RootItems { get { return _root; } }

        public FileViewModel CurrentFile { get; private set; }

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
        }
    }
}

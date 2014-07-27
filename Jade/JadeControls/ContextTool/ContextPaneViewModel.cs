using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeControls.ContextTool
{
    public class ContextPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.IEditorController _editorController;
        private CppCodeBrowser.IProjectIndex _currentIndex;

        private JadeCore.CppSymbols2.ISymbolTable _st = new JadeCore.CppSymbols2.TranslationUnitTable();
        
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
                SelectPath("main.cpp/Test/Overloads/Func");
            }
        }

        private void UpdateTree(FilePath path)
        {
            string selectedPath = "";
            CppCodeBrowser.ISourceFile sf = _currentIndex.FindSourceFile(path);
            if(CurrentFile != null)
            {
                ITreeItem selected = CurrentFile.FindSelected();
                if (selected != null)
                    selectedPath = selected.TreeItemPath;
            }
            _st.Update(sf.TranslationUnit);
            CurrentFile = new FileViewModel(path, sf.TranslationUnit);

            if(selectedPath.Length > 0)
            {
                SelectPath(selectedPath);
            }

            OnPropertyChanged("RootItems");
        }

        public ObservableCollection<ITreeItem> RootItems { get { return CurrentFile == null ? null : CurrentFile.Children; } }

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

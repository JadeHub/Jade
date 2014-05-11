using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using CppCodeBrowser;

namespace JadeControls.EditorControl.ViewModel
{
    using JadeCore;

    /// <summary>
    /// View model for the entire Editor Control. Manages the collection of tabs and associated DocumentViewModels
    /// </summary>
    public class EditorControlViewModel : JadeControls.NotifyPropertyChanged
    {
        #region Data

        private ObservableCollection<DocumentViewModel> _documents;
        private JadeCore.IEditorController _controller;
        private DocumentViewModel _selectedDocument;

        #endregion

        #region Constructor

        public EditorControlViewModel(JadeCore.IEditorController controller)
        {
            //Bind to the Model
            _controller = controller;
            _controller.ActiveDocumentChanged += OnControllerActiveDocumentChanged;

            _documents = new ObservableCollection<DocumentViewModel>();
        }

        #endregion

        #region Model event handlers

        private void OnDocumentClosing(DocumentViewModel vm)
        {
            if (_documents.Contains(vm))
            {
                _documents.Remove(vm);
            }
        }

        private void OnControllerActiveDocumentChanged(JadeCore.EditorDocChangeEventArgs args)
        {
            DocumentViewModel vm = GetViewModel(args.Document);
            if (vm == null && args.Document != null)
            {
                vm = new SourceDocumentViewModel(args.Document);
                _documents.Add(vm);
                //change to IEditorController.DocumentClosed
                args.Document.OnClosing += delegate { OnDocumentClosing(vm); };
            }
            SelectedDocument = vm;

        }
                
        #endregion

        #region Public Properties
           
        public ObservableCollection<DocumentViewModel> Documents { get { return _documents; } }
        
        public DocumentViewModel SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                if (_selectedDocument != value)
                {
                    if (_selectedDocument != null)
                    {
                        _selectedDocument.IsSelected = false;
                    }
                    _selectedDocument = value;
                    if (_selectedDocument != null)
                    {
                        _controller.ActiveDocument = _selectedDocument.Document;
                        _selectedDocument.IsSelected = true;
                    }
                    OnPropertyChanged("SelectedDocument");
                }
            }
        }

        #endregion

        #region Command Handlers

        public void OnClose(object param)
        {
            if(param != null && param is DocumentViewModel)
            {
                DocumentViewModel doc = param as DocumentViewModel;
            }
        }
                
        public bool CanDoClose()
        {
            return SelectedDocument != null;
        }

        #endregion

        #region Private Methods

        public DocumentViewModel GetViewModel(JadeCore.IEditorDoc doc)
        {
            foreach (DocumentViewModel vm in _documents)
            {
                if (vm.Document == doc)
                    return vm;
            }
            return null;
        }
        
        #endregion
    }
}

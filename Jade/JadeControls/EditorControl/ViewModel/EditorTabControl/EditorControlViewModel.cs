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
        private IDocumentViewModelFactory _docViewModelFactory;

        #endregion

        #region Constructor

        public EditorControlViewModel(JadeCore.IEditorController controller, IDocumentViewModelFactory docViewModelFactory)
        {
            //Bind to the Model
            _controller = controller;
            _controller.ActiveDocumentChanged += OnControllerActiveDocumentChanged;
            _controller.DocumentClosed += OnControllerDocumentClosed;
            _documents = new ObservableCollection<DocumentViewModel>();
            _docViewModelFactory = docViewModelFactory;
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
                
        private void OnControllerActiveDocumentChanged(IEditorDoc newValue, IEditorDoc oldValue)
        {
            DocumentViewModel vm = FindViewModel(newValue);
            if (vm == null && newValue != null)
            {
                vm = _docViewModelFactory.Create(newValue);
                _documents.Add(vm);
            }
            SelectedDocument = vm;
        }

        private void OnControllerDocumentClosed(EditorDocChangeEventArgs args)
        {
            IEditorDoc doc = args.Document;

            foreach(DocumentViewModel vm in _documents)
            {
                if(vm.Document == doc)
                {
                    _documents.Remove(vm);
                    break;
                }
            }
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

        public DocumentViewModel FindViewModel(JadeCore.IEditorDoc doc)
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

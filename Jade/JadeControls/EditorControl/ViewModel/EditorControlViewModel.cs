using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    internal class DocumentCollection
    {
        private ObservableCollection<DocumentViewModel> _documents;

        internal DocumentCollection()
        {
            _documents = new ObservableCollection<DocumentViewModel>();
            _documents.CollectionChanged += OnOpenDocumentsChanged;
        }

        internal void Add(DocumentViewModel doc)
        {
            _documents.Add(doc);
        }

        internal bool Contains(string displayName)
        {
            foreach (DocumentViewModel d in _documents)
                if (d.DisplayName == displayName)
                    return true;
            return false;
        }

        private void OnOpenDocumentsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (DocumentViewModel doc in e.NewItems)
                    doc.RequestClose += this.OnDocumentRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (DocumentViewModel doc in e.OldItems)
                    doc.RequestClose -= this.OnDocumentRequestClose;
        }

        private void OnDocumentRequestClose(object sender, EventArgs e)
        {
            DocumentViewModel doc = sender as DocumentViewModel;
            _documents.Remove(doc);
            
        }

        public ObservableCollection<DocumentViewModel> Documents
        {
            get
            {
                return _documents;
            }
        }

        public void Close(FilePath path)
        {
            foreach (DocumentViewModel doc in _documents)
            {
                if (doc.Path.Equals(path))
                {
                    _documents.Remove(doc);
                    break;
                }
            }
        }
    }

    public class EditorControlViewModel : JadeControls.NotifyPropertyChanged
    {
        #region Data

        private DocumentCollection _openDocuments;
        private DocumentViewModel _selectedDocument;
        private JadeCore.IEditorController _controller;

        #endregion

        public EditorControlViewModel(JadeCore.IEditorController controller)
        {
            _controller = controller;
            _controller.DocumentOpened += OnDocumentOpened;
            _controller.DocumentClosed += OnDocumentClosed;
            _openDocuments = new DocumentCollection();
        }

        void OnDocumentClosed(JadeCore.EditorDocChangeEventArgs args)
        {
            _openDocuments.Close(args.Document.Path);                
        }

        void OnDocumentOpened(JadeCore.EditorDocChangeEventArgs args)
        {
            DocumentViewModel d = new DocumentViewModel(args.Document);
            _openDocuments.Add(d);
            SelectedDocument = d;
        }

        #region Public Properties

        public ObservableCollection<DocumentViewModel> OpenDocuments
        {
            get { return _openDocuments.Documents; }
        }

        public DocumentViewModel SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                if (_selectedDocument != null)
                    _selectedDocument.Selected = false;
                _selectedDocument = value;
                if (_selectedDocument != null)
                    _selectedDocument.Selected = true;
                OnPropertyChanged("SelectedDocument");
            }
        }

        #endregion
    }
}

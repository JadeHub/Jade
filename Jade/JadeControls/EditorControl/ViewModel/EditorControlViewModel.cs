using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JadeControls.EditorControl.ViewModel
{
    internal class DocumentCollection
    {
        private ObservableCollection<JadeCore.ViewModels.IEditorDocument> _documents;

        internal DocumentCollection()
        {
            _documents = new ObservableCollection<JadeCore.ViewModels.IEditorDocument>();
            _documents.CollectionChanged += OnOpenDocumentsChanged;
        }

        internal void Add(JadeCore.ViewModels.IEditorDocument doc)
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
            doc.Dispose();
        }

        public ObservableCollection<JadeCore.ViewModels.IEditorDocument> Documents
        {
            get
            {
                return _documents;
            }
        }

        public void CloseAll()
        {
            JadeCore.ViewModels.IEditorDocument[] tmp = _documents.ToArray();

            foreach (DocumentViewModel doc in tmp)
            {
                _documents.Remove(doc);
                doc.Dispose();
            }
        }
    }

    public class EditorControlViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IEditorViewModel
    {
        #region Data

        private DocumentCollection _openDocuments;
        private DocumentViewModel _selectedDocument;

        #endregion

        public EditorControlViewModel()
        {
            _openDocuments = new DocumentCollection();
          /*  _openDocuments.Add(new DocumentViewModel("test1", "11111"));
            _openDocuments.Add(new DocumentViewModel("test2", "22222"));
            _selectedDocument = _openDocuments.Documents.ElementAt(0);
            _selectedDocument.Selected = true;*/
        }

        public void OpenSourceFile(JadeUtils.IO.IFileHandle handle)
        {
            DocumentViewModel d = new DocumentViewModel(handle.Name, handle.Path.Str);
            _openDocuments.Add(d);
            SelectedDocument = d;
        }

        public void CloseAllDocuments()
        {
            _openDocuments.CloseAll();
        }

        #region Public Properties

        public ObservableCollection<JadeCore.ViewModels.IEditorDocument> OpenDocuments
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

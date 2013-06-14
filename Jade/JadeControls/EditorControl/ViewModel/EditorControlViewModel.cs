using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JadeControls.EditorControl.ViewModel
{
    public class DocumentViewModel
    {
        #region Data

        private string _name;
        private string _text;

        #endregion

        public DocumentViewModel(string name, string text)
        {
            _name = name;
            _text = text;
        }

        #region Public Properties

        public string DisplayName { get { return _name;}}
        public string Text { get { return _text; } set { _text = value; } }

        #endregion

        #region Close Command

        private RelayCommand _closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param => this.OnCloseCommand(), param => this.CanDoCloseCommand);
                }
                return _closeCommand;
            }
        }

        public void OnCloseCommand()
        {
            EventHandler h = RequestClose;
            if (h != null)
                h(this, EventArgs.Empty);            
        }

        private bool CanDoCloseCommand
        {
            get { return true; }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;

        #endregion
    }

    public class EditorControlViewModel
    {
        #region Data

        private ObservableCollection<DocumentViewModel> _openDocuments;

        #endregion

        public EditorControlViewModel()
        {
            _openDocuments = new ObservableCollection<DocumentViewModel>();
            _openDocuments.CollectionChanged += OnOpenDocumentsChanged;
            _openDocuments.Add(new DocumentViewModel("Test1", "111"));
            _openDocuments.Add(new DocumentViewModel("Test2", "222"));
        }

        void OnOpenDocumentsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (DocumentViewModel doc in e.NewItems)
                    doc.RequestClose += this.OnDocumentRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (DocumentViewModel doc in e.OldItems)
                    doc.RequestClose -= this.OnDocumentRequestClose;
        }

        void OnDocumentRequestClose(object sender, EventArgs e)
        {
            DocumentViewModel doc = sender as DocumentViewModel;
         //   doc.Dispose();
            this.OpenDocuments.Remove(doc);
        }

        #region Public Properties

        public ObservableCollection<DocumentViewModel> OpenDocuments
        {
            get { return _openDocuments; }
        }

        #endregion
    }
}

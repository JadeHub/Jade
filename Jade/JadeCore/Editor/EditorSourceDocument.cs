using JadeUtils.IO;
using System;
using System.Threading;
using System.IO;


namespace JadeCore.Editor
{
    public interface ISourceDocument : JadeCore.IEditorDoc
    {
        event EventHandler OnIndexUpdated;
        CppCodeBrowser.IProjectIndex ProjectIndex { get; }
    }
        
    public class SourceDocument : ISourceDocument
    {
        #region Data

        private IEditorController _controller;
        private ITextDocument _document;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;
        private SourceDocumentParseThread _parseThread;
        
        #endregion

        #region Constructor

        public SourceDocument(IEditorController controller, ITextDocument document, CppCodeBrowser.IIndexBuilder ib)
        {
            _controller = controller;
            _document = document;
            _indexBuilder = ib;
            _parseThread = new SourceDocumentParseThread(this, _indexBuilder, (bool b) => { RaiseOnIndexUpdated(); });
            _parseThread.HighPriority = controller.ActiveDocument == this;
            _parseThread.Run = true;
            _controller.ActiveDocumentChanged += OnActiveDocumentChanged;
        }

        void OnActiveDocumentChanged(EditorDocChangeEventArgs args)
        {
            _parseThread.HighPriority = args.Document == this;
            _controller.ActiveDocumentChanged -= OnActiveDocumentChanged;
        }

        public void Dispose()
        {
            _parseThread.Dispose();
        }

        #endregion

        #region IEditorDoc implementation

        public event EventHandler OnIndexUpdated;

        private void RaiseOnIndexUpdated()
        {
            EventHandler handler = OnIndexUpdated;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler OnClosing;

        private void RaiseOnClosing()
        {
            EventHandler handler = OnClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
                
        public void Close()
        {
            RaiseOnClosing();
        }

        public void Save()
        {
            _document.Save(_document.File);
        }
                
        public ITextDocument TextDocument { get { return _document; } }
        
        public string Name
        {
            get { return _document.Name; }
        }

        public IFileHandle File
        {
            get { return _document.File; }
        }

        public bool Modified
        {
            get
            {
                return _document.Modified;
            }            
        }

        #endregion

        #region IEditorSourceDocument implementation

        public CppCodeBrowser.IProjectIndex ProjectIndex
        {
            get { return _indexBuilder.Index; }
        }

        #endregion
    }
}

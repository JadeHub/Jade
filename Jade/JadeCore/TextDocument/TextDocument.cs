using JadeUtils.IO;
using System;
using System.IO;

namespace JadeCore
{
    public class TextDocument : ITextDocument
    {
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;
        private bool _loaded;

        public TextDocument(IFileHandle handle)
        {
            File = handle;
            _loaded = false;
            _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument();
        }

        #region Properties

        public string Name
        {
            get { return File.Name; }
        }

        public IFileHandle File
        {
            get;
            private set;
        }

        public string Content
        {
            get 
            {
                if (!_loaded)
                {
                    Load();
                }
                return _avDoc.Text;
            }
        }

        #endregion

        #region Private Methods

        private void Load()
        {
            using (FileStream fs = new FileStream(File.Path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.ASCII, false))
                {
                    _avDoc.Text = reader.ReadToEnd();
                    _loaded = true;
                }
            }
        }

        #endregion

        public void AddFileObserver(IFileObserver observer) { }
        public void RemoveFileObserver(IFileObserver observer) { }
    }
}

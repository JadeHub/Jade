using System;
using System.Diagnostics;
using System.IO;
using JadeUtils.IO;

namespace JadeCore.Editor
{
    public class EditorSourceDocument : JadeCore.IEditorDoc
    {
        #region Data

        private IFileHandle _file;
        private string _content;
        private bool _modified;

        #endregion

        #region Constructor

        public EditorSourceDocument(IFileHandle file)
        {
            _file = file;
        }

        #endregion

        #region Events

        public event EventHandler OnClosing;

        private void RaiseOnClosing()
        {
            EventHandler handler = OnClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        public void Close()
        {
            RaiseOnClosing();
        }

        #region Public Properties

        public string Name
        {
            get { return _file.Name; }
        }

        public FilePath Path
        {
            get { return _file.Path; }
        }

        public bool Modified
        {
            get
            {
                return _modified;
            }
            set { _modified = value; }
        }

        public string Content
        {
            get
            {
                if (_content == null)
                {
                    Load();
                }
                return _content;
            }

            set
            {
                Modified = true;
                _content = value;
            }
        }

        #endregion

        #region Private Methods

        private void Load()
        {
            using (FileStream fs = new FileStream(Path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    _content = reader.ReadToEnd();
                }
            }
        }

        #endregion
    }
}

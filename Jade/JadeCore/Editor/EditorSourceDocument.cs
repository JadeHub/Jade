using JadeUtils.IO;
using System;
using System.IO;

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
        public event EventHandler OnSaved;

        private void RaiseOnClosing()
        {
            EventHandler handler = OnClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RaiseOnSaved()
        {
            EventHandler handler = OnSaved;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            RaiseOnClosing();
        }

        public void Save()
        {
            using (FileStream fs = new FileStream(Path.Str, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    System.Diagnostics.Debug.Write(_content);
                    writer.Write(_content);                    
                    Modified = false;
                    RaiseOnSaved();
                }
            }
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get { return _file.Name; }
        }

        public FilePath Path
        {
            get { return _file.Path; }
        }

        public IFileHandle File
        {
            get { return _file; }
        }

        public bool Modified
        {
            get
            {
                return _modified;
            }
            set 
            { 
                _modified = value; 
            }
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
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.ASCII, false))
                { 
                    _content = reader.ReadToEnd();
                }
            }
        }

        #endregion
    }
}

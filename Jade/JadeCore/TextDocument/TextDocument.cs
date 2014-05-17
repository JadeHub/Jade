﻿using JadeUtils.IO;
using System;
using System.IO;
using ICSharpCode.AvalonEdit.Document;

namespace JadeCore
{
    public class TextDocument : ITextDocument
    {
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;
        private bool _loaded;
        private bool _modified;
        private UInt64 _version;

        public TextDocument(IFileHandle handle)
        {
            File = handle;
            _loaded = false;
            _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument();
            _avDoc.TextChanged += OnAvDocTextChanged;
            _modified = false;
            _version = 0;
        }

        public event EventHandler TextChanged
        {
            add {_avDoc.TextChanged += value;}
            remove { _avDoc.TextChanged -= value; }
        }

        public event EventHandler ModifiedChanged;

        private void RaiseModifiedChangedEvent()
        {
            EventHandler handler = ModifiedChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnAvDocTextChanged(object sender, EventArgs e)
        {
            _version++;
            Modified = true;
        }

        #region Properties

        public ICSharpCode.AvalonEdit.Document.TextDocument AvDoc 
        { 
            get 
            {
                if (!_loaded)
                    Load();
                return _avDoc; 
            }
        }

        public string Name
        {
            get { return File.Name; }
        }

        public IFileHandle File
        {
            get;
            private set;
        }

        public int TextLength 
        {
            get { return AvDoc.TextLength; }
        }

        public bool Modified 
        {
            get { return _modified; }
            private set
            {
                if(_modified != value)
                {
                    _modified = value;
                    RaiseModifiedChangedEvent();
                }
            }
        }

        public UInt64 Version 
        {
            get { return _version; }
        }

        #endregion

        #region Public Methods

        public void DiscardChanges()
        {
            if (!_modified || !_loaded)
                return;
            Load();
        }

        public bool Save(IFileHandle file)
        {
            if (!_loaded || !_modified)
                return false;

            using (FileStream fs = new FileStream(file.Path.Str, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    System.Diagnostics.Debug.Write(AvDoc.Text);
                    writer.Write(AvDoc.Text);
                    Modified = false;
                }
            }
            File = file;
            return true;
        }

        public int GetLineNumForOffset(int offset)
        {
            return AvDoc.GetLineByOffset(offset).LineNumber;
        }
        
        public ISegment GetLineForOffset(int offset)
        {
            return AvDoc.GetLineByOffset(offset);
        }

        public string GetText(ISegment segment)
        {
            return AvDoc.GetText(segment);
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
                    Modified = false;
                }
            }
        }
                
        #endregion
    }
}

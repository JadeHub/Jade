﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class DocumentViewModel : ViewModelBase
    {
        #region Data

        private IEditorDoc _document;
        private bool _selected;
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;

        #endregion

        #region Constructor

        public DocumentViewModel(IEditorDoc doc)
        {
            _document = doc;
            _document.OnSaved += delegate { OnPropertyChanged("Modified"); };
            _selected = false;
        }

        #endregion

        #region Public Properties

        public string Path { get { return _document.Path.Str; } }
        
        public override string DisplayName { get { return _document.Name; } }

        public bool Modified
        {
            get { return _document.Modified; }

            set
            {
                if (_document.Modified != value)
                {
                    _document.Modified = value;
                    OnPropertyChanged("Modified");
                }
            }
        }

        public bool Selected 
        { 
            get 
            { 
                return _selected; 
            } 
            set 
            { 
                _selected = value;
                if (_selected && _avDoc == null)
                {
                    _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument(_document.Content);
                    _avDoc.TextChanged += _avDoc_TextChanged;
                }
                OnPropertyChanged("Selected");
            } 
        }

        void _avDoc_TextChanged(object sender, EventArgs e)
        {
            bool m = Modified;
            _document.Content = _avDoc.Text;
            if(!m)
                OnPropertyChanged("Modified");
        }

        public ICSharpCode.AvalonEdit.Document.TextDocument TextDocument
        {
            get { return _avDoc; }
        }

        public IEditorDoc Document
        {
            get { return _document; }
        }

        #endregion        
        
    }
}
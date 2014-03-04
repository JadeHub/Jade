using JadeCore;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace JadeControls.EditorControl.ViewModel
{    
    //wrapps an IEditorDoc and associated CodeEditor view 
    public abstract class DocumentViewModel : Docking.PaneViewModel
    {
        #region Data

        static ImageSourceConverter ISC = new ImageSourceConverter();
               
        private bool _selected;

        /// <summary>
        /// The doc from the EditorController
        /// </summary>
        private IEditorDoc _model;
        private CodeEditor _view;

        //The view's view of the content
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;

        /// <summary>
        /// Current location of cursor
        /// </summary>
        private int _caretOffset;
                
        #endregion

        #region Constructor

        protected DocumentViewModel(IEditorDoc doc)
        {
            Title = doc.Name;
            ContentId = doc.Path.Str;
            IconSource = ISC.ConvertFromInvariantString("pack://application:,,,/Images/File.png") as ImageSource;
            _model = doc;
            _model.OnSaved += delegate { OnPropertyChanged("Modified"); };
            _selected = false;
        }

        public virtual void SetView(CodeEditor view)
        {
            _view = view;
            _avDoc = _view.Document;

            RegisterCommands(_view.CommandBindings);

            //Load the document
            _avDoc.Text = _model.Content;
            _avDoc.TextChanged += _avDoc_TextChanged;
            
            //Initialise the view's caret location in case CaretOffset has already been set.
            _view.CaretOffset = _caretOffset;
            //Track changes in the caret location
            _view.TextArea.Caret.PositionChanged += Caret_PositionChanged;

            //Let any derived class initialise
            OnSetView(_view);
        }

        protected virtual void OnSetView(CodeEditor view)
        {
        }

        #endregion

        #region Public Properties

        public string Path { get { return _model.Path.Str; } }

        public bool Modified
        {
            get { return _model.Modified; }
            set
            {
                if (_model.Modified != value)
                {
                    _model.Modified = value;
                    OnPropertyChanged("Modified");
                }
            }
        }

        public bool Selected 
        { 
            get { return _selected; } 
            set 
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            } 
        }

        public int CaretOffset
        {
            get { return _caretOffset; }
            set
            {
                if (_caretOffset != value)
                {
                    _caretOffset = value;
                    if (_view != null)
                        _view.CaretOffset = _caretOffset;
                    OnPropertyChanged("CaretOffset");
                }
            }
        }
       
        public ICSharpCode.AvalonEdit.Document.TextDocument TextDocument
        {
            get { return _avDoc; }
        }

        public IEditorDoc Document
        {
            get { return _model; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display the specified location
        /// </summary>
        /// <param name="loc"></param>
        public void DisplayLocation(int offset, bool setFocus)
        {
            CaretOffset = offset;
            if (_view != null && setFocus)
                _view.TextArea.Focus();
        }

        public void HighlightRange(int startOffset, int endOffset)
        {
            _view.Select(startOffset, endOffset - startOffset);
        }

        #endregion

        #region Document Event Handlers

        private void _avDoc_TextChanged(object sender, EventArgs e)
        {
            bool m = Modified;
            _model.Content = _avDoc.Text;
            if (!m)
                OnPropertyChanged("Modified");
        }

        #endregion

        #region View Event Handlers

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (_caretOffset != _view.TextArea.Caret.Offset)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Line {0} Col {1} Offset{2}",
                                                _view.TextArea.Caret.Line,
                                                _view.TextArea.Caret.Column,
                                                _view.TextArea.Caret.Offset));
                _caretOffset = _view.TextArea.Caret.Offset;
                OnPropertyChanged("CaretOffset");
            }
        }

        #endregion

        public abstract void RegisterCommands(CommandBindingCollection commandBindings);
    }
}

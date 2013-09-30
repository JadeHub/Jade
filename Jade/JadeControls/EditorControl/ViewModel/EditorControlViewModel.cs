using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class EditorControlCommandAdaptor
    {
        private delegate void OnCommandDel(object parameter);
        private delegate bool CanDoCommandDel();

        private EditorControlViewModel _vm;

        public EditorControlCommandAdaptor(EditorControlViewModel vm)
        {
            _vm = vm;
        }

        public void Bind(CommandBindingCollection bindings)
        {
            Register(bindings, ApplicationCommands.Close, delegate(object param) { _vm.OnClose(param);}, delegate {return _vm.CanDoClose();});
        }

        private void Register(CommandBindingCollection bindings, ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
        {
            bindings.Add(new CommandBinding(command,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                            onCmd(args.Parameter);
                                            args.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs args)
                                        {
                                            args.CanExecute = canDoCmd();
                                            args.Handled = true;
                                        }));
        }
    }

    public class EditorControlViewModel : JadeControls.NotifyPropertyChanged
    {
        #region Data

        private ObservableCollection<EditorTabItem> _tabItems;

        private EditorTabItem _selectedDocument;
        private JadeCore.IEditorController _controller;
        private EditorControlCommandAdaptor _commands;

        #endregion

        #region Constructor

        public EditorControlViewModel(JadeCore.IEditorController controller)
        {
            //Bind to the Model
            _controller = controller;
            _controller.DocumentOpened += OnModelDocumentOpened;
            _controller.DocumentClosed += OnModelDocumentClosed;

            //Setup Command Adaptor
            _commands = new EditorControlCommandAdaptor(this);

            _tabItems = new ObservableCollection<EditorTabItem>();            
        }

        #endregion

        #region Model event handlers

        private void OnModelDocumentClosed(JadeCore.EditorDocChangeEventArgs args)
        {
            
        }

        private void OnModelDocumentOpened(JadeCore.EditorDocChangeEventArgs args)
        {
            DocumentViewModel d = new DocumentViewModel(args.Document);
            EditorTabItem view = new EditorTabItem();
            view.DataContext = d;
            _tabItems.Add(view);
            SelectedDocument = view;
            OnPropertyChanged("TabItems");
        }

        #endregion

        #region Public Properties

        public EditorControlCommandAdaptor Commands { get { return _commands; } }
                
        public ObservableCollection<EditorTabItem> TabItems { get { return _tabItems; } }

        public EditorTabItem SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                if (_selectedDocument != null)
                {
                    DocumentViewModel vm = _selectedDocument.DataContext as DocumentViewModel;
                    vm.Selected = false;
                }
                _selectedDocument = value;
                if (_selectedDocument != null)
                {
                    DocumentViewModel vm = _selectedDocument.DataContext as DocumentViewModel;
                    vm.Selected = true;

                    _controller.ActiveDocument = vm.Document;
                }
                else
                {
                    _controller.ActiveDocument = null;
                }
                
                OnPropertyChanged("SelectedDocument");
            }
        }

        #endregion

        #region Command Handlers

        public void OnClose(object param)
        {
            if(param != null && param is DocumentViewModel)
            {
                DocumentViewModel doc = param as DocumentViewModel;
            }
        }

        public bool CanDoClose()
        {
            return true;
        }

        #endregion
    }
}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels
{
    public class JadeCommandAdaptor
    {
        private delegate void OnCommandDel(object parameter);
        private delegate bool CanDoCommandDel();
        
        private JadeCore.IJadeCommandHandler _handler;

        public JadeCommandAdaptor(JadeCore.IJadeCommandHandler handler)
        {
            _handler = handler;
        }

        public void Bind(CommandBindingCollection bindings)
        {
            Register(bindings, JadeCore.Commands.OpenDocument, delegate(object param) { _handler.OnOpenDocument(param as JadeUtils.IO.IFileHandle); });

            Register(bindings, JadeCore.Commands.Exit, delegate { _handler.OnExit(); });
            Register(bindings, JadeCore.Commands.NewWorkspace, delegate { _handler.OnNewWorkspace(); });
            Register(bindings, JadeCore.Commands.CloseWorkspace, delegate { _handler.OnCloseWorkspace(); }, delegate { return _handler.CanCloseWorkspace(); });
            Register(bindings, JadeCore.Commands.OpenWorkspace, delegate { _handler.OnOpenWorkspace(); }, delegate { return _handler.CanOpenWorkspace(); });
            Register(bindings, JadeCore.Commands.SaveWorkspace, delegate { _handler.OnSaveWorkspace(); }, delegate { return _handler.CanSaveWorkspace(); });
            Register(bindings, JadeCore.Commands.SaveAsWorkspace, delegate { _handler.OnSaveAsWorkspace(); }, delegate { return _handler.CanSaveAsWorkspace(); });

            Register(bindings, JadeCore.Commands.ViewLineNumbers, delegate { _handler.OnViewLineNumbers(); }, delegate { return _handler.CanViewLineNumbers(); });
            Register(bindings, JadeCore.Commands.CloseAllDocuments, delegate { _handler.OnCloseAllDocuments(); }, delegate { return _handler.CanCloseAllDocuments(); });
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

        private void Register(CommandBindingCollection bindings, ICommand command, OnCommandDel onCmd)
        {
            bindings.Add(new CommandBinding(command,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                            onCmd(args.Parameter);
                                            args.Handled = true;
                                        }));
        }
    }

}

using System;
using System.Windows.Input;

namespace JadeGui.ViewModels
{
    public class JadeCommandAdaptor
    {
        private delegate void OnCommandDel(object parameter);
        private delegate bool CanDoCommandDel();
        
        private JadeCore.ViewModels.IJadeViewModel _vm;

        public JadeCommandAdaptor(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _vm = vm;
        }

        public void Bind(CommandBindingCollection bindings)
        {
            Register(bindings, JadeCore.Commands.OpenDocument, delegate(object param) { _vm.OnOpenDocument(param as JadeUtils.IO.IFileHandle); });

            Register(bindings, JadeCore.Commands.Exit, delegate { _vm.OnExit(); });
            Register(bindings, JadeCore.Commands.NewWorkspace, delegate { _vm.OnNewWorkspace(); });
            Register(bindings, JadeCore.Commands.CloseWorkspace, delegate { _vm.OnCloseWorkspace(); }, delegate { return _vm.CanCloseWorkspace(); });
            Register(bindings, JadeCore.Commands.OpenWorkspace, delegate { _vm.OnOpenWorkspace(); }, delegate { return _vm.CanOpenWorkspace(); });
            Register(bindings, JadeCore.Commands.SaveWorkspace, delegate { _vm.OnSaveWorkspace(); }, delegate { return _vm.CanSaveWorkspace(); });
            Register(bindings, JadeCore.Commands.SaveAsWorkspace, delegate { _vm.OnSaveAsWorkspace(); }, delegate { return _vm.CanSaveAsWorkspace(); });

            Register(bindings, JadeCore.Commands.ViewLineNumbers, delegate { _vm.OnViewLineNumbers(); }, delegate { return _vm.CanViewLineNumbers(); });
            Register(bindings, JadeCore.Commands.CloseAllDocuments, delegate { _vm.OnCloseAllDocuments(); }, delegate { return _vm.CanCloseAllDocuments(); });
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

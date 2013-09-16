using System;
using System.Windows.Input;

namespace JadeGui.ViewModels
{
    public static class JadeCommands
    {
        public static readonly RoutedCommand Exit = new RoutedCommand("Exit", typeof(MainWindow));
        public static readonly RoutedCommand NewWorkspace = new RoutedCommand("NewWorkspace", typeof(MainWindow));
        public static readonly RoutedCommand CloseWorkspace = new RoutedCommand("CloseWorkspace", typeof(MainWindow));
        public static readonly RoutedCommand OpenWorkspace = new RoutedCommand("OpenWorkspace", typeof(MainWindow));
        public static readonly RoutedCommand SaveWorkspace = new RoutedCommand("SaveWorkspace", typeof(MainWindow));
        public static readonly RoutedCommand SaveAsWorkspace = new RoutedCommand("SaveAsWorkspace", typeof(MainWindow));
    }

    public class JadeCommandAdaptor
    {
        private delegate void OnCommandDel();
        private delegate bool CanDoCommandDel();
        
        private JadeCore.ViewModels.IJadeViewModel _vm;       

        public JadeCommandAdaptor(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _vm = vm;
        }

        public void Bind(CommandBindingCollection bindings)
        {
            Register(bindings, JadeCommands.Exit, delegate { _vm.OnExit(); });            
            Register(bindings, JadeCommands.NewWorkspace, delegate { _vm.OnNewWorkspace(); });
            Register(bindings, JadeCommands.CloseWorkspace, delegate { _vm.OnCloseWorkspace(); }, delegate { return _vm.CanCloseWorkspace(); });
            Register(bindings, JadeCommands.OpenWorkspace, delegate { _vm.OnOpenWorkspace(); }, delegate { return _vm.CanOpenWorkspace(); });
            Register(bindings, JadeCommands.SaveWorkspace, delegate { _vm.OnSaveWorkspace(); }, delegate { return _vm.CanSaveWorkspace(); });
            Register(bindings, JadeCommands.SaveAsWorkspace, delegate { _vm.OnSaveAsWorkspace(); }, delegate { return _vm.CanSaveAsWorkspace(); });
        }

        private void Register(CommandBindingCollection bindings, ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
        {
            bindings.Add(new CommandBinding(command,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                            onCmd();
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
                                            onCmd();
                                            args.Handled = true;
                                        }));
        }
    }

}

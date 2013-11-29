using System;
using System.Windows.Input;

namespace JadeControls.EditorControl
{
    public class EditorCommands
    {
        public static readonly RoutedCommand GoToDefinition = new RoutedCommand("GoToDefinition", typeof(object));        
    }

    public class EditorControlCommandAdaptor
    {
        private delegate void OnCommandDel(object parameter);
        private delegate bool CanDoCommandDel();

        private ViewModel.EditorControlViewModel _vm;

        public EditorControlCommandAdaptor(ViewModel.EditorControlViewModel vm)
        {
            _vm = vm;
        }

        public void Bind(CommandBindingCollection bindings)
        {
            Register(bindings, ApplicationCommands.Close, delegate(object param) { _vm.OnClose(param); }, delegate { return _vm.CanDoClose(); });
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
}

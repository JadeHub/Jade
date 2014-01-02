using JadeCore;
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
            Register(bindings, Commands.OpenDocument,       delegate(object param) { _handler.OnOpenDocument(param as JadeUtils.IO.IFileHandle); });

            Register(bindings, ApplicationCommands.New,     delegate { _handler.OnNewFile(); },         delegate { return _handler.CanNewFile(); });
            Register(bindings, ApplicationCommands.Open,    delegate { _handler.OnOpenFile(); },        delegate { return _handler.CanOpenFile(); });
            Register(bindings, ApplicationCommands.Save,    delegate { _handler.OnSaveFile(); },        delegate { return _handler.CanSaveFile(); });
            Register(bindings, ApplicationCommands.SaveAs,  delegate { _handler.OnSaveAsFile(); },      delegate { return _handler.CanSaveAsFile(); });
            Register(bindings, Commands.SaveAllFiles,       delegate { _handler.OnSaveAllFiles(); },    delegate { return _handler.CanSaveAllFiles(); });
            Register(bindings, Commands.CloseFile,          delegate { _handler.OnCloseFile(); },       delegate { return _handler.CanCloseFile(); });

            Register(bindings, Commands.Exit,               delegate { _handler.OnExit(); });
            Register(bindings, Commands.NewWorkspace,       delegate { _handler.OnNewWorkspace(); });
            Register(bindings, Commands.CloseWorkspace,     delegate { _handler.OnCloseWorkspace(); },  delegate { return _handler.CanCloseWorkspace(); });
            Register(bindings, Commands.PromptOpenWorkspace, delegate { _handler.OnPromptOpenWorkspace(); }, delegate { return _handler.CanPromptOpenWorkspace(); });
            Register(bindings, Commands.OpenWorkspace,      delegate(object param) { _handler.OnOpenWorkspace(param as string); },   delegate { return _handler.CanOpenWorkspace(); });
            Register(bindings, Commands.SaveWorkspace,      delegate { _handler.OnSaveWorkspace(); },   delegate { return _handler.CanSaveWorkspace(); });
            Register(bindings, Commands.SaveAsWorkspace,    delegate { _handler.OnSaveAsWorkspace(); }, delegate { return _handler.CanSaveAsWorkspace(); });

            Register(bindings, Commands.ViewLineNumbers,    delegate { _handler.OnViewLineNumbers(); }, delegate { return _handler.CanViewLineNumbers(); });

            Register(bindings, Commands.CloseAllDocuments,  delegate { _handler.OnCloseAllDocuments(); }, delegate { return _handler.CanCloseAllDocuments(); });

            Register(bindings, Commands.DisplayCodeLocation,  delegate { _handler.OnDisplayCodeLocation(null); }, delegate { return true; });
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

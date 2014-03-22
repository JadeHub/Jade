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

        public void Bind(CommandBindingCollection commandBindings)
        {
            Register(commandBindings, Commands.OpenDocument,       delegate(object param) 
                                                                            { 
                                                                                _handler.OnOpenDocument(param as JadeCore.OpenFileCommandParams); 
                                                                            });

            Register(commandBindings, ApplicationCommands.New,     delegate { _handler.OnNewFile(); },         delegate { return _handler.CanNewFile(); });
            Register(commandBindings, ApplicationCommands.Open,    delegate(object param)
                                                                            { 
                                                                                _handler.OnOpenFile(param as JadeUtils.IO.IFileHandle); 
                                                                            },        delegate { return _handler.CanOpenFile(); });

            Register(commandBindings, ApplicationCommands.Save,    delegate { _handler.OnSaveFile(); },        delegate { return _handler.CanSaveFile(); });
            Register(commandBindings, ApplicationCommands.SaveAs,  delegate { _handler.OnSaveAsFile(); },      delegate { return _handler.CanSaveAsFile(); });
            Register(commandBindings, Commands.SaveAllFiles,       delegate { _handler.OnSaveAllFiles(); },    delegate { return _handler.CanSaveAllFiles(); });
            Register(commandBindings, Commands.CloseFile,          delegate { _handler.OnCloseFile(); },       delegate { return _handler.CanCloseFile(); });

            Register(commandBindings, Commands.Exit,               delegate { _handler.OnExit(); });
            Register(commandBindings, Commands.NewWorkspace,       delegate { _handler.OnNewWorkspace(); });
            Register(commandBindings, Commands.CloseWorkspace,     delegate { _handler.OnCloseWorkspace(); },  delegate { return _handler.CanCloseWorkspace(); });
            Register(commandBindings, Commands.PromptOpenWorkspace, delegate { _handler.OnPromptOpenWorkspace(); }, delegate { return _handler.CanPromptOpenWorkspace(); });
            Register(commandBindings, Commands.OpenWorkspace,      delegate(object param) { _handler.OnOpenWorkspace(param as string); },   delegate { return _handler.CanOpenWorkspace(); });
            Register(commandBindings, Commands.SaveWorkspace,      delegate { _handler.OnSaveWorkspace(); },   delegate { return _handler.CanSaveWorkspace(); });
            Register(commandBindings, Commands.SaveAsWorkspace,    delegate { _handler.OnSaveAsWorkspace(); }, delegate { return _handler.CanSaveAsWorkspace(); });

            Register(commandBindings, Commands.ViewLineNumbers,    delegate { _handler.OnViewLineNumbers(); }, delegate { return _handler.CanViewLineNumbers(); });

            Register(commandBindings, Commands.CloseAllDocuments,  delegate { _handler.OnCloseAllDocuments(); }, delegate { return _handler.CanCloseAllDocuments(); });

            Register(commandBindings, Commands.SearchFile, delegate { _handler.OnSearchFile(); }, delegate { return _handler.CanSearchFile(); });
            Register(commandBindings, Commands.SearchInFiles, delegate { _handler.OnSearchInFiles(); }, delegate { return _handler.CanSearchInFiles(); });
            Register(commandBindings, Commands.SearchDisplayNext, delegate { _handler.OnDisplayNextSearchResult(); }, delegate { return _handler.CanDisplayNextSearchResult(); });
            Register(commandBindings, Commands.SearchDisplayPrev, delegate { _handler.OnDisplayPrevSearchResult(); }, delegate { return _handler.CanDisplayPrevSearchResult(); });
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

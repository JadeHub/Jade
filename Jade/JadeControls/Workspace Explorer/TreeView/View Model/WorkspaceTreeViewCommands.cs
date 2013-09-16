using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;

namespace JadeControls.Workspace.ViewModel
{
    public static class TreeViewCommands
    {
        public static readonly RoutedCommand OpenDocument = new RoutedCommand("OpenDocument", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddFolder = new RoutedCommand("AddFolder", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddProject = new RoutedCommand("AddProject", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddFile= new RoutedCommand("AddFile", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddExistingFile = new RoutedCommand("AddExistingFile", typeof(WorkspaceTreeView));

        public static readonly RoutedCommand RemoveProject = new RoutedCommand("RemoveProject", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand RemoveFolder = new RoutedCommand("RemoveFolder", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand RemoveFile = new RoutedCommand("RemoveFile", typeof(WorkspaceTreeView));
    }

    internal static class TreeViewCommandAdaptor
    {
        public static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();

        private delegate void OnCommandDel(WorkspaceTree vm);
        private delegate bool CanDoCommandDel(WorkspaceTree vm);

        static TreeViewCommandAdaptor()
        {
            Register(TreeViewCommands.OpenDocument, delegate(WorkspaceTree vm) { vm.OnOpenDocument(); }, delegate(WorkspaceTree vm) { return vm.CanOpenDocument(); });
            Register(TreeViewCommands.AddFolder, delegate(WorkspaceTree vm) { vm.OnAddFolder(); }, delegate(WorkspaceTree vm) { return vm.CanAddFolder(); });
            Register(TreeViewCommands.AddProject, delegate(WorkspaceTree vm) { vm.OnAddProject(); }, delegate(WorkspaceTree vm) { return vm.CanAddProject(); });
            Register(TreeViewCommands.AddFile, delegate(WorkspaceTree vm) { vm.OnAddFile(); }, delegate(WorkspaceTree vm) { return vm.CanAddFile(); });

            Register(TreeViewCommands.RemoveFolder, delegate(WorkspaceTree vm) { vm.OnRemoveFolder(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveFolder(); });
            Register(TreeViewCommands.RemoveProject, delegate(WorkspaceTree vm) { vm.OnRemoveProject(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveProject(); });
            Register(TreeViewCommands.RemoveFile, delegate(WorkspaceTree vm) { vm.OnRemoveFile(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveFile(); });
        }

        static private void Register(ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
        {
            CommandBindings.Add(new CommandBinding(command,
                                                            delegate(object target, ExecutedRoutedEventArgs args)
                                                            {
                                                                onCmd(GetVMFromTarget(target));

                                                            },
                                                            delegate(object target, CanExecuteRoutedEventArgs args)
                                                            {
                                                                args.CanExecute = canDoCmd(GetVMFromTarget(target));
                                                                args.Handled = true;
                                                            }));
        }
        static private WorkspaceTree GetVMFromTarget(object target)
        {
            if (target != null)
            {
                WorkspaceTreeView tv = target as WorkspaceTreeView;
                if(tv.DataContext != null && tv.DataContext.GetType() == typeof(WorkspaceTree))
                {
                    return tv.DataContext as WorkspaceTree;
                }
            }
            throw new Exception("Workspace TreeView command fired without VM.");
        }
    }      
}

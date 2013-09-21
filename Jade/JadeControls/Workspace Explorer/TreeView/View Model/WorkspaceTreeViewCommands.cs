using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;

namespace JadeControls.Workspace.ViewModel
{
    public static class TreeViewCommands
    {       
        public static readonly RoutedCommand AddFolder = new RoutedCommand("AddFolder", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddProject = new RoutedCommand("AddProject", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand CreateFile= new RoutedCommand("CreateFile", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand AddExistingFile = new RoutedCommand("AddExistingFile", typeof(WorkspaceTreeView));

        public static readonly RoutedCommand RemoveProject = new RoutedCommand("RemoveProject", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand RemoveFolder = new RoutedCommand("RemoveFolder", typeof(WorkspaceTreeView));
        public static readonly RoutedCommand RemoveFile = new RoutedCommand("RemoveFile", typeof(WorkspaceTreeView));
    }

    internal static class TreeViewCommandAdaptor
    {
        public static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();

        private delegate void OnCommandDel(WorkspaceTree vm, object param);
        private delegate bool CanDoCommandDel(WorkspaceTree vm, object param);

        static TreeViewCommandAdaptor()
        {
            Register(TreeViewCommands.AddFolder, delegate(WorkspaceTree vm, object param) { vm.OnAddFolder(); }, delegate(WorkspaceTree vm, object param) { return vm.CanAddFolder(); });
            Register(TreeViewCommands.AddProject, delegate(WorkspaceTree vm, object param) { vm.OnAddProject(); }, delegate(WorkspaceTree vm, object param) { return vm.CanAddProject(); });
            Register(TreeViewCommands.AddExistingFile, delegate(WorkspaceTree vm, object param) { vm.OnAddFile(param as ProjectFolder); }, delegate(WorkspaceTree vm, object param) { return vm.CanAddFile(); });

            Register(ApplicationCommands.Delete, delegate(WorkspaceTree vm, object param) { vm.OnRemoveItem(param); }, delegate(WorkspaceTree vm, object param) { return vm.CanRemoveItem(param); });
        }

        static private void Register(ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
        {
            CommandBindings.Add(new CommandBinding(command,
                                                delegate(object target, ExecutedRoutedEventArgs args)
                                                {
                                                    onCmd(GetVMFromTarget(target), args.Parameter);
                                                    args.Handled = true;

                                                },
                                                delegate(object target, CanExecuteRoutedEventArgs args)
                                                {
                                                    args.CanExecute = canDoCmd(GetVMFromTarget(target), args.Parameter);
                                                    args.Handled = true;
                                                }));
        }

        static private WorkspaceTree GetVMFromTarget(object target)
        {
            if (target != null)
            {
                WorkspaceTreeView tv = target as WorkspaceTreeView;
                WorkspaceViewModel wvm = tv.DataContext as WorkspaceViewModel;
                if (wvm != null)
                {
                    return wvm.Tree as WorkspaceTree;
                }
            }
            throw new Exception("Workspace TreeView command fired without VM.");
        }
    }      
}

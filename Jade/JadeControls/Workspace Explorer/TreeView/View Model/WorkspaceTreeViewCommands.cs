using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;

namespace JadeControls.Workspace.ViewModel
{
    /*
    public class UICommandAdaptor<VMType, ViewType> where ViewType : System.Windows.FrameworkElement where VMType : class
    {
        public static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();

        public delegate void OnCommandDel(VMType vm);
        public delegate bool CanDoCommandDel(VMType vm);

        public void Register(ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
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

        static private VMType GetVMFromTarget(object target)
        {
            if (target != null)
            {
                ViewType view = target as ViewType;
                if (view != null && view.DataContext != null && view.DataContext.GetType() == typeof(VMType))
                {
                    return view.DataContext as VMType;
                }
            }
            throw new Exception("Workspace TreeView command fired without VM.");
        }
    }
    */

    public static class TreeViewCommands
    {
        public static readonly RoutedCommand OpenDocument = new RoutedCommand("OpenDocument", typeof(WorkspaceTreeView));
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

        private delegate void OnCommandDel(WorkspaceTree vm);
        private delegate bool CanDoCommandDel(WorkspaceTree vm);

        static TreeViewCommandAdaptor()
        {
            Register(TreeViewCommands.OpenDocument, delegate(WorkspaceTree vm) { vm.OnOpenDocument(); }, delegate(WorkspaceTree vm) { return vm.CanOpenDocument(); });
            Register(TreeViewCommands.AddFolder, delegate(WorkspaceTree vm) { vm.OnAddFolder(); }, delegate(WorkspaceTree vm) { return vm.CanAddFolder(); });
            Register(TreeViewCommands.AddProject, delegate(WorkspaceTree vm) { vm.OnAddProject(); }, delegate(WorkspaceTree vm) { return vm.CanAddProject(); });
            Register(TreeViewCommands.AddExistingFile, delegate(WorkspaceTree vm) { vm.OnAddFile(); }, delegate(WorkspaceTree vm) { return vm.CanAddFile(); });
            
            Register(TreeViewCommands.RemoveFolder, delegate(WorkspaceTree vm) { vm.OnRemoveItem(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveItem(); });
            Register(TreeViewCommands.RemoveProject, delegate(WorkspaceTree vm) { vm.OnRemoveItem(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveItem(); });
            Register(TreeViewCommands.RemoveFile, delegate(WorkspaceTree vm) { vm.OnRemoveItem(); }, delegate(WorkspaceTree vm) { return vm.CanRemoveItem(); });
        }

        static private void Register(ICommand command, OnCommandDel onCmd, CanDoCommandDel canDoCmd)
        {
            CommandBindings.Add(new CommandBinding(command,
                                                delegate(object target, ExecutedRoutedEventArgs args)
                                                {
                                                    onCmd(GetVMFromTarget(target));
                                                    args.Handled = true;

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

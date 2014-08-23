using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore
{
    public interface IServiceProvider
    {
        JadeCore.Properties.Settings Settings { get;}
        JadeUtils.IO.IFileService FileService { get; set; }
        JadeCore.Workspace.IWorkspaceController WorkspaceController { get; set; }
        JadeCore.IEditorController EditorController { get; set; }
        JadeCore.Output.IOutputController OutputController { get; set; }
        JadeCore.IJadeCommandHandler CommandHandler { get; set; }
        JadeCore.Search.ISearchController SearchController { get; set; }
        System.Windows.Window MainWindow { get; set; }
        TaskScheduler GuiScheduler { get; set; }
        JadeCore.Parsing.ParseController CppParser { get; set; }
    }

    public class Services
    {
        static IServiceProvider _serviceProvider;

        static Services()
        {
            _serviceProvider = new ServiceProvider();
        }

        public static IServiceProvider Provider { get { return _serviceProvider; } }    
    }

    public class ServiceProvider : IServiceProvider
    {
        public JadeCore.Properties.Settings Settings { get { return JadeCore.Properties.Settings.Default; } }
        public JadeUtils.IO.IFileService FileService { get; set; }
        public JadeCore.Workspace.IWorkspaceController WorkspaceController { get; set; }
        public JadeCore.IEditorController EditorController { get; set; }
        public JadeCore.Output.IOutputController OutputController { get; set; }
        public JadeCore.IJadeCommandHandler CommandHandler { get; set; }
        public JadeCore.Search.ISearchController SearchController { get; set; }
        public System.Windows.Window MainWindow { get; set; }
        public TaskScheduler GuiScheduler { get; set; }
        public JadeCore.Parsing.ParseController CppParser { get; set; }
    }
}

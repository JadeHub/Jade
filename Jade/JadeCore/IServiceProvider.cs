using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore
{
    public interface IServiceProvider
    {
        JadeCore.ViewModels.IJadeViewModel JadeViewModel { get; set; }
        JadeUtils.IO.IFileService FileService { get; set; }
        JadeCore.IWorkspaceController WorkspaceController { get; set; }
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
        public JadeCore.ViewModels.IJadeViewModel JadeViewModel { get; set; }
        public JadeUtils.IO.IFileService FileService { get; set; }
        public JadeCore.IWorkspaceController WorkspaceController { get; set; }
    }
}

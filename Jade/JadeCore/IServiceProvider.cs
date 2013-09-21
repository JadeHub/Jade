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

        IO.IFileService FileService { get; set; }
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
        private JadeCore.ViewModels.IJadeViewModel _jadeViewModel;

        public JadeCore.ViewModels.IJadeViewModel JadeViewModel
        {
            get { return _jadeViewModel; }
            set { _jadeViewModel = value; }
        }

        public IO.IFileService FileService { get; set; }
    }
}

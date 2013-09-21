using System;
using System.Windows.Input;

namespace JadeCore
{
    public static class Commands
    {
        public static readonly RoutedCommand OpenDocument = new RoutedCommand("OpenDocument", typeof(object));        
        public static readonly RoutedCommand NewWorkspace = new RoutedCommand("NewWorkspace", typeof(object));
        public static readonly RoutedCommand CloseWorkspace = new RoutedCommand("CloseWorkspace", typeof(object));
        public static readonly RoutedCommand OpenWorkspace = new RoutedCommand("OpenWorkspace", typeof(object));
        public static readonly RoutedCommand SaveWorkspace = new RoutedCommand("SaveWorkspace", typeof(object));
        public static readonly RoutedCommand SaveAsWorkspace = new RoutedCommand("SaveAsWorkspace", typeof(object));
        public static readonly RoutedCommand Exit = new RoutedCommand("Exit", typeof(object));

        public static readonly RoutedCommand ViewLineNumbers = new RoutedCommand("ViewLineNumbers", typeof(object));
        public static readonly RoutedCommand CloseAllDocuments = new RoutedCommand("CloseAllDocuments", typeof(object));
    }
}

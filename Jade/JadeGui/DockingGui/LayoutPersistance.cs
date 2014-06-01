using System;
using System.IO;

namespace JadeGui.DockingGui
{
    using JadeUtils.IO;
    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;

    static class LayoutReaderWriter
    {
        public static void Write(DockingManager dockingManager, string fileName)
        {
          /*  var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new StreamWriter(fileName))
                serializer.Serialize(stream);*/
        }

        public static void Read(DockingManager dockingManager, string fileName)
        {
           /* if (File.Exists(fileName) == false) return; 
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new StreamReader(fileName))
                serializer.Deserialize(stream);*/
        }
    }
}

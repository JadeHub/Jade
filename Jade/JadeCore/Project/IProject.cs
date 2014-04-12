using System.Collections.Generic;
using System.Linq;
using CppCodeBrowser;
using JadeUtils.IO;

namespace JadeCore.Project
{
    public interface IProject : IFolder
    {
        string Path { get; }
        string Directory { get; }

        CppCodeBrowser.IProjectIndex SourceIndex { get; }

        FileItem FindFile(FilePath path);

        void OnItemAdded(IItem item);
        void OnItemRemoved(IItem item);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Workspace
{
    public interface INewWorkspace
    {
        string Name { get; }
        FilePath Path { get; }
        Collections.Observable.List<Project.IProject> Projects { get; }
        Collections.Observable.List<string> Groups { get; }
        void AddProject(string group, Project.IProject p);
        void AddGroup(string group);
    }

    public interface IWorkspace : IFolder
    {
        string Path { get; set; }
        string Directory { get; }

        Collections.Observable.List<Project.IProject> AllProjects { get; }

        ISet<FilePath> Files { get; }
    }
}

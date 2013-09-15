using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.ViewModels
{
    public interface IEditorDocument
    {
        string DisplayName
        {
            get;
        }
    }

    public interface IEditorViewModel
    {
        void OpenSourceFile(JadeData.Project.File file);
    }

    public interface IWorkspaceViewModel
    {
        string Path { get; set; }
        string Directory { get; }
        bool Modified { get; set; }


    }

    public interface IJadeViewModel
    {
        IWorkspaceViewModel Workspace
        {
            get;
      //      set;
        }

        IEditorViewModel Editor
        {
            get;
        }

        IWorkspaceManager WorkspaceManager { get; }

        /*bool SaveWorkspace(string path);
        bool CloseWorkspace();*/
    }
}

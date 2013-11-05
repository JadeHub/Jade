using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JadeData.Persistence
{
 /*   static public class Writer
    {
        static private ProjectFileType MakeProjectFile(JadeData.Project.File file)
        {
            ProjectFileType result = new ProjectFileType();
            result.Name = file.Name;
            result.Path = file.Path;
            return result;
        }

        static private ProjectFolderType MakeProjectFolder(JadeData.Project.IFolder folder)
        {
            ProjectFolderType result = new ProjectFolderType();

            result.Name = folder.Name;
            result.Files = new ProjectFileType[folder.Items.OfType<Project.File>().Count()];
            result.Folders = new ProjectFolderType[folder.Folders.Count];

            int idx = 0;
            foreach (Project.File f in folder.Items.OfType<Project.File>())
            {
                result.Files[idx] = MakeProjectFile(f);
                idx++;
            }
            idx = 0;
            foreach (Project.IFolder f in folder.Folders)
            {
                result.Folders[idx] = MakeProjectFolder(f);
            }
            return result;
        }

        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj)
        {
            ProjectType result = new ProjectType();

            result.Name = proj.Name;
            result.Path = proj.Path;
            result.Files = new ProjectFileType[proj.Items.OfType<Project.File>().Count()];
            result.Folders = new ProjectFolderType[proj.Folders.Count];
            
            int idx=0;
            foreach (Project.File f in proj.Items.OfType<Project.File>())
            {
                result.Files[idx] = MakeProjectFile(f);
                idx++;
            }
            idx = 0;
            foreach (Project.IFolder f in proj.Folders)
            {
                result.Folders[idx] = MakeProjectFolder(f);
            }
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i]);
            }
            result.Folders = subs;

            List<Workspace.ProjectItem> projTemps = new List<Workspace.ProjectItem>();
            foreach (Workspace.ProjectItem proj in folder.Items.OfType<Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i]);
            }
            result.Projects = projs;

            return result;
        }

        static public void Write(JadeData.Workspace.IWorkspace workspace)
        {
            
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            JadeData.Workspace.IFolder folder = workspace;
            Workspace.WorkspaceType result = new 
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i]);
            }
            result.Folders = subs;

            List<Workspace.ProjectItem> projTemps = new List<Workspace.ProjectItem>();
            foreach (Workspace.ProjectItem proj in folder.Items.OfType<Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i]);
            }
            result.Projects = projs;

            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path); 
            try
            {                    
                serializer.Serialize(stream, result);
                stream.Close();
                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }*/
}

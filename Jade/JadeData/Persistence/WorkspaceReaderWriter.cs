using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JadeCore.IO;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(string workspaceDir, ProjectType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                //Convert from relative path stored in workspace xml file
                path = JadeCore.IO.Path.CombinePaths(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(IFileHandle file)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(file.Path.Str);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, file.Path);
            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            //Convert to relative path for storage in workspace
            result.Path = JadeCore.IO.Path.CalculateRelativePath(workspaceDir + @"\", proj.Path);
            Persistence.Project.Writer.Write(proj, proj.Path);
            return result;
        }

        static private FolderType MakeFolder(JadeData.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for(int i=0;i<projTemps.Count;i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
            }
            result.Projects = projs;

            return result;
        }

        static private string GetProjectPath(JadeData.Workspace.IWorkspace workspace, JadeData.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
            {
                projTemps.Add(proj);
            }
            ProjectType[] projs = new ProjectType[projTemps.Count];
            for (int i = 0; i < projTemps.Count; i++)
            {
                projs[i] = MakeProject(projTemps[i], workspaceDir);
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
    }
}

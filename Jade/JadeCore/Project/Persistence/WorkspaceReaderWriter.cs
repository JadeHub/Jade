using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JadeUtils.IO;

namespace JadeCore.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeCore.Project.IProject MakeProject(string workspaceDir, ProjectType xml, IFileService fileService)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                //Convert from relative path stored in workspace xml file
                path = JadeUtils.IO.Path.CombinePaths(workspaceDir, path);
            }
            return Persistence.Project.Reader.Read(path, fileService);
        }

        private static JadeCore.Workspace.IFolder MakeFolder(string workspaceDir, FolderType xml, IFileService fileService)
        {
            JadeCore.Workspace.IFolder result = new JadeCore.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(workspaceDir, f, fileService));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(workspaceDir, p, fileService));
            }
            return result;
        }

        static public JadeCore.Workspace.IWorkspace Read(IFileHandle file, IFileService fileService)
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

            JadeCore.Workspace.IWorkspace result = new JadeCore.Workspace.Workspace(xml.Name, file);
            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f, fileService));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(result.Directory, p, fileService));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeCore.Workspace.ProjectItem proj, string workspaceDir)
        {
            ProjectType result = new ProjectType();

            //Convert to relative path for storage in workspace
            result.Path = JadeUtils.IO.Path.CalculateRelativePath(workspaceDir + @"\", proj.Path);
            Persistence.Project.Writer.Write(proj, proj.Path);
            return result;
        }

        static private FolderType MakeFolder(JadeCore.Workspace.IFolder folder, string workspaceDir)
        {
            FolderType result = new FolderType();
            result.Name = folder.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeCore.Workspace.ProjectItem> projTemps = new List<JadeCore.Workspace.ProjectItem>();
            foreach (JadeCore.Workspace.ProjectItem proj in folder.Items.OfType<JadeCore.Workspace.ProjectItem>())
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

        static private string GetProjectPath(JadeCore.Workspace.IWorkspace workspace, JadeCore.Project.IProject proj)
        {
            return "";
        }

        static public string Write(JadeCore.Workspace.IWorkspace workspace, string path)
        {
            string workspaceDir = System.IO.Path.GetDirectoryName(path);

            JadeCore.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i], workspaceDir);
            }
            result.Folders = subs;

            List<JadeCore.Workspace.ProjectItem> projTemps = new List<JadeCore.Workspace.ProjectItem>();
            foreach (JadeCore.Workspace.ProjectItem proj in folder.Items.OfType<JadeCore.Workspace.ProjectItem>())
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JadeData.Persistence.Workspace
{
    public static class Reader
    {
        private static JadeData.Project.IProject MakeProject(ProjectType xml)
        {
            /*JadeData.Project.Project proj = new JadeData.Project.Project(xml.Name, xml.Path);

            foreach (ProjectFileType f in xml.Files)
                proj.AddItem(MakeProjectFile(f));

            foreach (ProjectFolderType f in xml.Folders)
                proj.AddFolder(MakeProjectFolder(f));

            return proj;*/
            return Persistence.Project.Reader.Read(xml.Path);
        }

        private static JadeData.Workspace.IFolder MakeFolder(FolderType xml)
        {
            JadeData.Workspace.IFolder result = new JadeData.Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(p));
            }
            return result;
        }

        static public JadeData.Workspace.IWorkspace Read(string path)
        {
            WorkspaceType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            try
            {                
                xml = (WorkspaceType)serializer.Deserialize(tr);                
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }
            JadeData.Workspace.IWorkspace result = new JadeData.Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(f));
            }
            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(p));
            }

            return result;
        }
    }

    public static class Writer
    {
        static private ProjectType MakeProject(JadeData.Workspace.ProjectItem proj)
        {
            ProjectType result = new ProjectType();

            result.Path = proj.Path;
            Persistence.Project.Writer.Write(proj, proj.Path);
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

            List<JadeData.Workspace.ProjectItem> projTemps = new List<JadeData.Workspace.ProjectItem>();
            foreach (JadeData.Workspace.ProjectItem proj in folder.Items.OfType<JadeData.Workspace.ProjectItem>())
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

        static public string Write(JadeData.Workspace.IWorkspace workspace, string path)
        {
            JadeData.Workspace.IFolder folder = workspace;
            WorkspaceType result = new WorkspaceType();

            result.Name = workspace.Name;

            FolderType[] subs = new FolderType[folder.Folders.Count];
            for (int i = 0; i < folder.Folders.Count; i++)
            {
                subs[i] = MakeFolder(folder.Folders[i]);
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
    }
}

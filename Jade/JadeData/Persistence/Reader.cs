using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace JadeData.Persistence
{
    /*
    static public class Reader
    {
        private static Project.IItem MakeProjectFile(ProjectFileType xml)
        {
            return new Project.File(xml.Name, xml.Path);
        }

        private static Project.IFolder MakeProjectFolder(ProjectFolderType xml)
        {
            Project.Folder folder = new Project.Folder(xml.Name);

            foreach (ProjectFileType f in xml.Files)
                folder.AddItem(MakeProjectFile(f));

            foreach (ProjectFolderType f in xml.Folders)
                folder.AddFolder(MakeProjectFolder(f));

            return folder;
        }

        private static Project.IProject MakeProject(ProjectType xml)
        {
            Project.Project proj = new Project.Project(xml.Name, xml.Path);

            foreach (ProjectFileType f in xml.Files)
                proj.AddItem(MakeProjectFile(f));

            foreach (ProjectFolderType f in xml.Folders)
                proj.AddFolder(MakeProjectFolder(f));
    
            return proj;
        }

        private static Workspace.IFolder MakeWorkpaceFolder(FolderType xml)
        {
            Workspace.IFolder result = new Workspace.Folder(xml.Name);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeWorkpaceFolder(f));
            }

            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(p));
            }

            return result;
        }

        public static Workspace.IWorkspace ReadWorkspace(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceType));
            TextReader tr = new StreamReader(path);
            WorkspaceType xml = (WorkspaceType)serializer.Deserialize(tr);
            tr.Close();

            Workspace.IWorkspace result = new Workspace.Workspace(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeWorkpaceFolder(f));
            }

            foreach (ProjectType p in xml.Projects)
            {
                result.AddProject(MakeProject(p));
            }
            return result;
        }
    }*/
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;

namespace JadeData.Persistence.Project
{
    public static class Reader
    {
        private static JadeData.Project.IItem MakeFile(string projectDir, FileType xml)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(projectDir, path);
            }
            return new JadeData.Project.File(xml.Name, path);
        }

        private static JadeData.Project.IFolder MakeFolder(string projectDir, FolderType xml)
        {
            JadeData.Project.Folder folder = new JadeData.Project.Folder(xml.Name);

            foreach (FileType f in xml.Files)
                folder.AddItem(MakeFile(projectDir, f));

            foreach (FolderType f in xml.Folders)
                folder.AddFolder(MakeFolder(projectDir, f));

            return folder;
        }

        public static JadeData.Project.IProject Read(string path)
        {
            ProjectType xml;
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectType));
            TextReader tr = new StreamReader(path);
            try
            {
                xml = (ProjectType)serializer.Deserialize(tr);
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }
            JadeData.Project.IProject result = new JadeData.Project.Project(xml.Name, path);

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result.Directory, f));
            }
            foreach (FileType f in xml.Files)
            {
                result.AddItem(MakeFile(result.Directory, f));
            }
            return result;
        }
    }

    public static class Writer
    {
        static private FileType MakeFile(JadeData.Project.File file, string projectDir)
        {
            /*string absPath = file.Path;

            if (System.IO.Path.IsPathRooted(absPath) == false)
            {
                absPath = System.IO.Path.Combine(projectDir, absPath);
            }*/

            FileType result = new FileType();
            result.Name = file.Name;
            result.Path = file.Path;
            return result;
        }

        static private FolderType MakeFolder(JadeData.Project.IFolder folder, string projectDir)
        {
            FolderType result = new FolderType();

            result.Name = folder.Name;
            result.Files = new FileType[folder.Items.OfType<JadeData.Project.File>().Count()];
            result.Folders = new FolderType[folder.Folders.Count];

            int idx = 0;
            foreach (JadeData.Project.File f in folder.Items.OfType<JadeData.Project.File>())
            {
                result.Files[idx] = MakeFile(f, projectDir);
                idx++;
            }
            idx = 0;
            foreach (JadeData.Project.IFolder f in folder.Folders)
            {
                result.Folders[idx] = MakeFolder(f, projectDir);
            }
            return result;
        }
        static public void Write(JadeData.Project.IProject project, string path)
        {
            string projectDir = System.IO.Path.GetDirectoryName(path);

            ProjectType result = new ProjectType();

            result.Name = project.Name;
            result.Files = new FileType[project.Items.OfType<JadeData.Project.File>().Count()];
            result.Folders = new FolderType[project.Folders.Count];

            int idx = 0;
            foreach (JadeData.Project.File f in project.Items.OfType<JadeData.Project.File>())
            {
                result.Files[idx] = MakeFile(f, projectDir);
                idx++;
            }
            idx = 0;
            foreach (JadeData.Project.IFolder f in project.Folders)
            {
                result.Folders[idx] = MakeFolder(f, projectDir);
            }
            
            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            System.IO.TextWriter stream = new System.IO.StreamWriter(path);
            try
            {
                serializer.Serialize(stream, result);
                stream.Close();
                return;
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

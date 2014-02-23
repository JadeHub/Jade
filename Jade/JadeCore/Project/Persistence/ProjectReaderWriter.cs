using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using JadeUtils.IO;

namespace JadeCore.Persistence.Project
{
    public static class Reader
    {
        private static JadeCore.Project.IItem MakeFile(string projectDir, FileType xml, IFileService fileService)
        {
            string path = xml.Path;
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                //Convert from relative path stored in project xml file
                path = System.IO.Path.Combine(projectDir, path);                
            }
            return new JadeCore.Project.File(fileService.MakeFileHandle(path));
        }

        private static JadeCore.Project.IFolder MakeFolder(JadeCore.Project.IProject project, string projectDir, FolderType xml, IFileService fileService)
        {
            JadeCore.Project.Folder folder = new JadeCore.Project.Folder(project, xml.Name);

            foreach (FileType f in xml.Files)
                folder.AddItem(MakeFile(projectDir, f, fileService));

            foreach (FolderType f in xml.Folders)
                folder.AddFolder(MakeFolder(project, projectDir, f, fileService));

            return folder;
        }

        public static JadeCore.Project.IProject Read(string path, IFileService fileService)
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

            JadeCore.Project.IProject result = new JadeCore.Project.Project(xml.Name, fileService.MakeFileHandle(path));

            foreach (FolderType f in xml.Folders)
            {
                result.AddFolder(MakeFolder(result, result.Directory, f, fileService));
            }
            foreach (FileType f in xml.Files)
            {
                result.AddItem(MakeFile(result.Directory, f, fileService));
            }
            return result;
        }
    }

    public static class Writer
    {
        static private FileType MakeFile(JadeCore.Project.File file, string projectDir)
        {
            FileType result = new FileType();
            //Convert to relative path for storage in workspace
            result.Path = JadeUtils.IO.PathUtils.CalculateRelativePath(projectDir + @"\", file.Path.Str);
            return result;
        }

        static private FolderType MakeFolder(JadeCore.Project.IFolder folder, string projectDir)
        {
            FolderType result = new FolderType();

            result.Name = folder.Name;
            result.Files = new FileType[folder.Items.OfType<JadeCore.Project.File>().Count()];
            result.Folders = new FolderType[folder.Folders.Count];

            int idx = 0;
            foreach (JadeCore.Project.File f in folder.Items.OfType<JadeCore.Project.File>())
            {
                result.Files[idx] = MakeFile(f, projectDir);
                idx++;
            }
            idx = 0;
            foreach (JadeCore.Project.IFolder f in folder.Folders)
            {
                result.Folders[idx] = MakeFolder(f, projectDir);
            }
            return result;
        }
        static public void Write(JadeCore.Project.IProject project, string path)
        {
            string projectDir = System.IO.Path.GetDirectoryName(path);

            ProjectType result = new ProjectType();

            result.Name = project.Name;
            result.Files = new FileType[project.Items.OfType<JadeCore.Project.File>().Count()];
            result.Folders = new FolderType[project.Folders.Count];

            int idx = 0;
            foreach (JadeCore.Project.File f in project.Items.OfType<JadeCore.Project.File>())
            {
                result.Files[idx] = MakeFile(f, projectDir);
                idx++;
            }
            idx = 0;
            foreach (JadeCore.Project.IFolder f in project.Folders)
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

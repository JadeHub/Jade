using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JadeUtils.IO;

namespace JadeCore.Persistence.Workspace.VisualStudioImport
{
    internal class ProjectFiltersFileReader
    {
        private FilePath _filtersFilePath;
        private IFileService _fileService;
        private JadeCore.Project.IProject _project;
        private static readonly XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

        public ProjectFiltersFileReader(string projectName, FilePath filtersFilePath, FilePath projectPath, IFileService fileService)
        {
            _filtersFilePath = filtersFilePath;
            _fileService = fileService;
            _project = new JadeCore.Project.Project(projectName, projectPath);
        }

        /*
         1) Find the Folder definitions
                <Filter Include="Source">
                    <UniqueIdentifier>{1a2da8c0-5f2c-4e2e-bd3f-3c94e4c7925b}</UniqueIdentifier>
                </Filter>
          
         2) then look for source files
                <ClCompile Include="..\test.cpp">
                    <Filter>Source</Filter>
                </ClCompile>
         3) then look for headers
                <ClInclude Include="..\template.h">
                    <Filter>Header Files</Filter>
                </ClInclude>
        */

        private JadeCore.Project.IFolder FindProjectFolder(JadeCore.Project.IFolder parent, string path)
        {
            string[] parts = path.Split('\\');
            if (parts.Length == 0) return null;

            JadeCore.Project.IFolder result = parent;
            foreach (string name in parts)
            {
                result = result.FindFolder(name);
                if (result == null)
                    return null;
            }
            return result;
        }

        private void AddFolder(string path)
        {
            string[] parts = path.Split('\\');

            JadeCore.Project.IFolder parent = _project;
            foreach(string name in parts)
            {
                JadeCore.Project.IFolder folder = parent.FindFolder(name);
                if(folder == null)
                {
                    folder = new JadeCore.Project.Folder(_project, name);
                    parent.AddFolder(folder);
                }                
                parent = folder;
            }
        }

        private void AddFile(string path, string folderPath)
        {
            JadeCore.Project.IFolder folder = folderPath != null ? FindProjectFolder(_project, folderPath) : null;
            
            path = JadeUtils.IO.PathUtils.CombinePaths(_project.Directory, path);

            _project.AddItem(folder, new JadeCore.Project.FileItem(_fileService.MakeFileHandle(path)));
            //folder.AddItem(new JadeCore.Project.FileItem(_fileService.MakeFileHandle(path)));
        }

        public JadeCore.Project.IProject ReadFiltersFile()
        {
            XDocument xdoc = XDocument.Load(_filtersFilePath.Str);
            
            //Folders
            IEnumerable<string> folders =  from el in xdoc.Root.Descendants(ns + "Filter") 
                                            where el.Elements(ns + "UniqueIdentifier").Count() > 0
                                            select el.Attribute("Include").Value;

            foreach(string folder in folders)
            {
                AddFolder(folder);
            }

            //Source files
            foreach(XElement sourceElem in xdoc.Root.Descendants(ns + "ClCompile"))
            {
                string path = sourceElem.Attribute("Include").Value;
                string folder = sourceElem.Elements(ns + "Filter").Count() > 0 ? sourceElem.Elements(ns + "Filter").First().Value : null;
                AddFile(path, folder);
            }

            //Header files
            foreach (XElement sourceElem in xdoc.Root.Descendants(ns + "ClInclude"))
            {
                string path = sourceElem.Attribute("Include").Value;
                string folder = sourceElem.Elements(ns + "Filter").Count() > 0 ? sourceElem.Elements(ns + "Filter").First().Value : null;
                AddFile(path, folder);
            }

            return _project;
        }
    }
}

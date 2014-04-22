using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JadeUtils.IO;
using JadeCore.Project;
using System.Xml.Linq;
using System.Linq;

namespace JadeCore.Persistence.Workspace.VisualStudioImport
{
    internal static class ProjectReader
    {
        //<ClCompile Include="..\main.cpp" />
        static private string _compileLineRegex = @"\<ClCompile Include=\""(.*?)\""";
        
        //<ClInclude Include="..\template.h" />
        static private string _includeLineRegex = @"\<ClInclude Include=\""(.*?)\""";
        
        static private JadeCore.Project.IProject Read(string name, StreamReader reader, FilePath projectPath, IFileService fileService)
        {
            IProject project = new JadeCore.Project.Project(name, projectPath);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                Match match = Regex.Match(line, _compileLineRegex);
                if (match.Success)
                {
                    string itemPath = match.Groups[1].Value;
                    itemPath = JadeUtils.IO.PathUtils.CombinePaths(projectPath.Directory, itemPath);
                    project.AddItem(null, new JadeCore.Project.FileItem(fileService.MakeFileHandle(itemPath)));
                    continue;
                }
                match = Regex.Match(line, _includeLineRegex);
                if (match.Success)
                {
                    string itemPath = match.Groups[1].Value;
                    itemPath = JadeUtils.IO.PathUtils.CombinePaths(projectPath.Directory, itemPath);
                    project.AddItem(null, new JadeCore.Project.FileItem(fileService.MakeFileHandle(itemPath)));
                    continue;
                }
            }            
            return project;
        }

        private static StreamReader CreateFileReader(FilePath path)
        {
            FileStream fs = new FileStream(path.Str, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(fs, System.Text.Encoding.ASCII, false);
        }
        
        public static JadeCore.Project.IProject Read(string name, FilePath path, IFileService fileService)
        {
            FilePath filterFilePath = FilePath.Make(path.Str + ".filters");

            if(filterFilePath.Exists)
            {
                ProjectFiltersFileReader reader = new ProjectFiltersFileReader(name, filterFilePath, path, fileService);
                return reader.ReadFiltersFile();
            }
            else
            {
                using (StreamReader reader = CreateFileReader(path))
                {
                    return Read(name, reader, path, fileService);
                }
            }
        }
    }
}

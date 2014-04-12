using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JadeUtils.IO;
using JadeCore.Project;

namespace JadeCore.Persistence.Workspace.VisualStudioImport
{
    internal static class ProjectReader
    {
        //<ClCompile Include="..\main.cpp" />
        static private string _compileLineRegex = @"\<ClCompile Include=\""(.*?)\"" \/\>";
        
        //<ClInclude Include="..\template.h" />
        static private string _includeLineRegex = @"\<ClInclude Include=\""(.*?)\"" \/\>";

        static private JadeCore.Project.IProject Read(string name, StreamReader reader, FilePath path, IFileService fileService)
        {
            IProject project = new JadeCore.Project.Project(name, path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                Match match = Regex.Match(line, _compileLineRegex);
                if (match.Success)
                {
                    string itemPath = match.Groups[1].Value;
                    itemPath = JadeUtils.IO.PathUtils.CombinePaths(path.Directory, itemPath);
                    project.AddItem(new JadeCore.Project.FileItem(fileService.MakeFileHandle(itemPath)));
                    continue;
                }
                match = Regex.Match(line, _includeLineRegex);
                if (match.Success)
                {
                    string itemPath = match.Groups[1].Value;
                    itemPath = JadeUtils.IO.PathUtils.CombinePaths(path.Directory, itemPath);
                    project.AddItem(new JadeCore.Project.FileItem(fileService.MakeFileHandle(itemPath)));
                    continue;
                }
            }            
            return project;
        }

        public static JadeCore.Project.IProject Read(string name, FilePath path, IFileService fileService)
        {
            using (FileStream fs = new FileStream(path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.ASCII, false))
                {
                    return Read(name, reader, path, fileService);
                }
            }
        }
    }
}

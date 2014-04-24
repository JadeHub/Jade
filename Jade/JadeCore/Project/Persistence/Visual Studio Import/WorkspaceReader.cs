using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using JadeUtils.IO;
using JadeCore.Project;
using System.Text.RegularExpressions;

namespace JadeCore.Persistence.Workspace.VisualStudioImport
{
    public static class Reader
    {        
        static private string CppProjectTypeGUID = "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942";
        static private string FolderProjectTypeGUID = "2150E333-8FDC-42A3-9474-1A3956D46DE8";

        //Project("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}") = "CppTest", "CppTest.vcxproj", "{98CC9031-B16D-453E-8EF3-1CE197823AF9}"
        static private string _projectLineRegex = @"Project\(\""\{(.{8}-.{4}-.{4}-.{4}-.{12})\}\""\) = \""(.*?)\"", \""(.*?)\"", \""\{(.{8}-.{4}-.{4}-.{4}-.{12})\}";

        //GlobalSection(NestedProjects) = preSolution
        static private string _beginNestedProjectsGlobalSectionRegex = @"GlobalSection\(NestedProjects\) = preSolution";

        //{1D05ACB0-A087-4689-B2D7-128EBDBDB03A} = {85D5EAC2-97DA-4D51-8CAB-4457701251AC}
        static private string _nestingRegex = @"\{(.{8}-.{4}-.{4}-.{4}-.{12})\} = \{(.{8}-.{4}-.{4}-.{4}-.{12})\}";

        //EndGlobalSection
        static private string _endGlobalSectionRegex = "EndGlobalSection";

        private class TempItem<T>
        {
            public bool IsRootItem;
            public readonly T Item;

            public TempItem(T item)
            {
                IsRootItem = true;
                Item = item;
            }
        }

        static private JadeCore.Workspace.IWorkspace Read(StreamReader reader, FilePath path, IFileService fileService)
        {
            Dictionary<Guid, TempItem<IProject>> projects = new Dictionary<Guid, TempItem<IProject>>();
            Dictionary<Guid, TempItem<JadeCore.Workspace.IFolder>> folders = new Dictionary<Guid, TempItem<JadeCore.Workspace.IFolder>>();

            bool inNestedProjects = false;

            JadeCore.Workspace.IWorkspace workspace = new JadeCore.Workspace.Workspace(path.FileName, path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                Match match = Regex.Match(line, _projectLineRegex);
                if (match.Success)
                {
                    string typeGuid = match.Groups[1].Value;
                    string name = match.Groups[2].Value;
                    string projectPath = match.Groups[3].Value;
                    string guid = match.Groups[4].Value;

                    if (typeGuid == CppProjectTypeGUID)
                    {
                        projectPath = JadeUtils.IO.PathUtils.CombinePaths(path.Directory, projectPath);
                        IProject project = ProjectReader.Read(name, FilePath.Make(projectPath), fileService);
                        
                        //Load project file
                        projects.Add(new Guid(match.Groups[4].Value), new TempItem<IProject>(project));
                    }
                    else if (typeGuid == FolderProjectTypeGUID)
                    {
                        JadeCore.Workspace.IFolder folder = new JadeCore.Workspace.Folder(name, workspace);
                        folders.Add(new Guid(match.Groups[4].Value), new TempItem<JadeCore.Workspace.IFolder>(folder));
                    }
                    continue;
                }

                if(Regex.IsMatch(line, _beginNestedProjectsGlobalSectionRegex))
                {
                    if (inNestedProjects)
                        throw new Exception("Mulitple NestedProjectsGlobalSection in .sln file");
                    inNestedProjects = true;
                    continue;
                }

                if(Regex.IsMatch(line, _endGlobalSectionRegex))
                {
                    inNestedProjects = false;
                    continue;
                }

                if (inNestedProjects)
                {
                    match = Regex.Match(line, _nestingRegex);
                    if (match.Success)
                    {
                        Guid sourceGuid = new Guid(match.Groups[1].Value);
                        Guid folderGuid = new Guid(match.Groups[2].Value);

                        TempItem<JadeCore.Workspace.IFolder> destFolder;

                        //No error if the destination folder is not found, just let the source item become a root item in the Wokspace
                        if (folders.TryGetValue(folderGuid, out destFolder))
                        {
                            //Find the source
                            TempItem<IProject> project;
                            if (projects.TryGetValue(sourceGuid, out project))
                            {
                                //Add project to folder
                                destFolder.Item.AddProject(project.Item);
                                project.IsRootItem = false;
                            }
                            else
                            {
                                TempItem<JadeCore.Workspace.IFolder> sourceFolder;
                                if (folders.TryGetValue(sourceGuid, out sourceFolder))
                                {
                                    //Add folder to folder
                                    destFolder.Item.AddFolder(sourceFolder.Item);
                                    sourceFolder.IsRootItem = false;
                                }
                            }
                        }
                    }
                }
            }

            if (inNestedProjects)
                throw new Exception("End of NestedProjectsGlobalSection missing from .sln file");



            //Add the root level projects
            foreach(TempItem<IProject> project in projects.Values)
            {
                if(project.IsRootItem)
                    workspace.AddProject(project.Item);
            }

            //Add the root level folders
            foreach(TempItem<JadeCore.Workspace.IFolder> folder in folders.Values)
            {
                if (folder.IsRootItem)
                    workspace.AddFolder(folder.Item);
            }

            return workspace;
        }

        static public JadeCore.Workspace.IWorkspace Read(FilePath path, IFileService fileService)
        {
            using (FileStream fs = new FileStream(path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.ASCII, false))
                {
                    return Read(reader, path, fileService);
                }
            }            
        }
    }
}

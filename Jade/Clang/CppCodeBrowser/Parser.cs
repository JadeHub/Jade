using System;
using System.Collections.Generic;
using System.Linq;
using LibClang;
using JadeUtils.IO;

namespace CppCodeBrowser
{
    public static class Parser
    {
        public static ParseResult Parse(ProjectIndex index, FilePath path, string[] compilerArgs, IUnsavedFileProvider unsavedFiles)
        {
            TranslationUnit tu = new TranslationUnit(index.LibClangIndex, path.Str);

            //Take a snapshot of the source
            IEnumerable<ParseFile> files = unsavedFiles.UnsavedFiles;
            List<Tuple<string, string>> unsavedList = new List<Tuple<string, string>>();
            foreach (var i in files)
            {
                unsavedList.Add(new Tuple<string, string>(i.Path.Str, i.Content));
            }
            if (tu.Parse(compilerArgs, unsavedList) == false)
            {
                tu.Dispose();
                return null;
            }
            return new ParseResult(path, files, tu); ;
        }
    }
}

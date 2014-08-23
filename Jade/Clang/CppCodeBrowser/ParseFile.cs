using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace CppCodeBrowser
{
    public class ParseFile
    {
        public ParseFile(FilePath path, UInt64 version, string content)
        {
            Path = path;
            Version = version;
            Content = content;
        }

        public FilePath Path { get; private set; }
        public UInt64 Version { get; private set; }
        public string Content { get; private set; }
    }
}

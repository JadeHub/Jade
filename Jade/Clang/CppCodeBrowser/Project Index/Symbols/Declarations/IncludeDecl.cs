using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;
using JadeUtils.IO;

namespace CppCodeBrowser.Symbols
{
    /// <summary>
    /// Special case of IDeclaration to represent an included file. One of these will exist for each file included in the project
    /// and each #include statement will refer to this 'Declaration'
    /// The included file's path is used as the Usr
    /// </summary>
    public class IncludeDecl : IDeclaration
    {
        private Cursor _cursor;
        private FilePath _includedFilePath;
        private ICodeLocation _location;

        public IncludeDecl(Cursor c, ISymbolTable table)            
        {
            _cursor = c;
            _includedFilePath = FilePath.Make(Cursor.IncludedFile.Name);
            _location = new CodeLocation(_includedFilePath.Str, 0);
        }

        public string Spelling 
        {
            get { return _includedFilePath.Str; }
        }

        public ICodeLocation Location { get { return _location; } }

        public int SpellingLength { get { return Spelling.Length; } }

        public Cursor Cursor { get { return _cursor; } }

        public EntityKind Kind { get { return EntityKind.Include; } }

        public string Usr { get { return _includedFilePath.Str; } }

        public FilePath Path { get { return _includedFilePath; } }
    }
}

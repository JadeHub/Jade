using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Editor
{
    public interface IDocChangeTracker
    {
    //    bool RequiresParse { get; }
        UInt64 Version { get; }
    }

    public class DocChangeTracker : IDocChangeTracker
    {
        private IEditorDoc _doc;
        private CppCodeBrowser.ProjectIndexBuilder _indexBuilder;
        
        public DocChangeTracker(IEditorDoc document, CppCodeBrowser.ProjectIndexBuilder indexBuilder)
        {
            _doc = document;
            _indexBuilder = indexBuilder;
            _doc.TextDocument.Changed += TextDocument_Changed;
        }

        private void TextDocument_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            if (_doc.File.Path.Extention.ToLower() == ".c" || _doc.File.Path.Extention.ToLower() == ".cpp" || _doc.File.Path.Extention.ToLower() == ".cc")
            {
                //if change warrants reparsing
                //initiate parse
                JadeCore.Services.Provider.CppParser.AddJob(Parsing.ParsePriority.Editing, new Parsing.ParseJob(_doc.File.Path, null, _indexBuilder));
            }
        }

        public UInt64 Version
        {
            get { return _doc.TextDocument.Version; }
        }
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using CppCodeBrowser;

namespace JadeControls.EditorControl.ViewModel
{
    using JadeCore;

    public interface IDocumentViewModelFactory
    {
        DocumentViewModel Create(IEditorDoc doc);
    }

    public class DocumentViewModelFactory : IDocumentViewModelFactory
    {
        public DocumentViewModelFactory() { }

        public DocumentViewModel Create(IEditorDoc doc)
        {            
            if (IsSourceFile(doc))
            {
                return new SourceDocumentViewModel(doc);
            }
            else if(IsHeaderFile(doc))
            {
                return new HeaderDocumentViewModel(doc);
            }
            return null;
        }

        static private bool IsHeaderFile(IEditorDoc doc)
        {
            return doc.File.Path.Extention.ToLower() == ".h" || doc.File.Path.Extention.Length == 0;
        }

        static private bool IsSourceFile(IEditorDoc doc)
        {
            string ext = doc.File.Path.Extention.ToLower();
            return ext == ".c" || ext == ".cc" || ext == ".cpp";
        }
    }
}

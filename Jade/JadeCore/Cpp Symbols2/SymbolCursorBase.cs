using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;
using ICSharpCode.AvalonEdit.Document;

namespace JadeCore.CppSymbols2
{
    public abstract class SymbolCursorBase : ISymbolCursor
    {
        private LibClang.Cursor _cursor;

        public SymbolCursorBase(LibClang.Cursor c)
        {
            _cursor = c;
        }

        public LibClang.Cursor Cursor
        {
            get { return _cursor; }
        }

        public virtual string Spelling
        {
            get { return Cursor.Spelling; }            
        }

        public virtual string SourceText
        {
            get { return GetCursorSourceText(); }
        }
        
        protected IEnumerable<T> GetType<T>(LibClang.CursorKind kind)
        {
            Debug.Assert(JadeCore.Services.Provider.SymbolCursorFactory.CanCreateKind(kind));
            return from c in _cursor.Children
                   where c.Kind == kind
                   select
                       (T)JadeCore.Services.Provider.SymbolCursorFactory.Create(c);
                       ;
        }

        protected string GetCursorSourceText()
        {
            IFileHandle file = Services.Provider.FileService.MakeFileHandle(Cursor.Location.File.Name);
            if (file == null)
                return null;

            ITextDocument doc = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file);
            if (doc == null)
                return null;

            ISegment line = doc.GetLineForOffset(Cursor.Location.Offset);
            if (line != null)
            {
                return doc.GetText(line).Trim();
            }
            return null;
        }
    }
}

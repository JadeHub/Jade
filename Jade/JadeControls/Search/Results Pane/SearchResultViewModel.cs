using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using JadeUtils.IO;
using System.Windows.Input;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchResultItemViewModel
    {
        private JadeCore.Search.ISearchResult _result;
        private string _summary;
        private IFileHandle _file;
        
        public SearchResultItemViewModel(JadeCore.Search.ISearchResult result)
        {
            _result = result;

            StringBuilder sb = new StringBuilder();

            sb.Append(result.Location.Path.Str);
            sb.Append(" - ");
            
            JadeCore.ITextDocument doc = JadeCore.Services.Provider.ContentProvider.OpenTextDocument(result.File);
            _file = doc.File;
            LineNum = doc.GetLineNumForOffset(result.Location.Offset);
            ISegment line = doc.GetLineForOffset(result.Location.Offset);
            if(line != null)
            {
                int column = result.Location.Offset - line.Offset + 1;
                sb.Append("(");
                sb.Append(LineNum);
                sb.Append(",");
                sb.Append(column);
                sb.Append(")");
                sb.Append(": ");
                sb.Append(doc.GetText(line).Trim());
            }
            _summary = sb.ToString();            
        }

        public string DisplayText
        {
            get
            {                
                return _summary;
            }
        }

        public JadeCore.Search.ISearchResult Result
        {
            get { return _result; }
        }

        public override string ToString()
        {
            return DisplayText;
        }

        public int LineNum
        {
            get;
            private set;
        }

        public IFileHandle File
        {
            get { return _file; }
        }        
    }
}

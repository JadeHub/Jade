using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CppCodeBrowser;

namespace JadeCore.Search
{
    public class TextDocumentSearch : SearchBase
    {
        private ITextDocument _document;
        private string _pattern;

        public TextDocumentSearch(ITextDocument document)
        {
            _document = document;
            _pattern = null;
        }

        protected override void DoSearch()
        {
            if (_pattern == null)
                return;

            Regex regex = new Regex(_pattern);

            MatchCollection matches = Regex.Matches(_document.AvDoc.Text, _pattern);
            foreach(Match match in matches)
            {
                ICodeLocation location = new CodeLocation(_document.File.Path.Str, match.Index);
                ISearchResult result = new CodeSearchResult(10, location, match.Length);
                AddResult(result);
            }
        }

        public string Pattern
        {
            get
            {
                return _pattern;
            }

            set
            {
                if (_pattern != value)
                {
                    _pattern = value;
                    Summary = _pattern;
                    Rerun();
                }
            }
        }

        public ITextDocument Document
        {
            get { return _document; }
        }
    }
}

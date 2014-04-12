using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CppCodeBrowser;

namespace JadeCore.Search
{
    class TextDocumentSearch : SearchBase
    {
        private ITextDocument _document;
        private string _pattern;

        public TextDocumentSearch(ITextDocument document, string regex)
        {
            _document = document;
            _pattern = regex;
        }

        protected override void DoSearch()
        {
            Regex regex = new Regex(_pattern);

            MatchCollection matches = Regex.Matches(_document.AvDoc.Text, _pattern);
            foreach(Match match in matches)
            {
                ICodeLocation location = new CodeLocation(_document.File.Path.Str, match.Index);
                ISearchResult result = new CodeSearchResult(10, location, match.Length);
                AddResult(result);
            }
        }
    }
}

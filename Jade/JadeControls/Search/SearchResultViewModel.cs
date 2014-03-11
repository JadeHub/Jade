using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchResultItemViewModel
    {
        private JadeCore.Search.ISearchResult _result;

        public SearchResultItemViewModel(JadeCore.Search.ISearchResult result)
        {
            
            _result = result;
        }

        public string DisplayText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_result.Summary);
                sb.Append(" ");
                sb.Append(_result.ToString());
                return sb.ToString();
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
    }
}

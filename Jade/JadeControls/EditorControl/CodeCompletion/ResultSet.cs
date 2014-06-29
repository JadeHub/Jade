using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace JadeControls.EditorControl.CodeCompletion
{
    public interface IResultSet { }

    public class ResultSet : IResultSet
    {
        HashSet<CompletionData> _results;

        public ResultSet()
        {
            _results = new HashSet<CompletionData>();
        }

        public void Add(CompletionData result)
        {
            if(_results.Add(result) == false)
            {
                //int i = 0;
            }
        }

        public IEnumerable<CompletionData> Results { get { return _results; } }
    }
}

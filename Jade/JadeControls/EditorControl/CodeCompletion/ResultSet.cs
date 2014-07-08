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
        Dictionary<Tuple<string, LibClang.CursorKind>, IResult> _items;

        public ResultSet()
        {
            _items = new Dictionary<Tuple<string,LibClang.CursorKind>,IResult>();
        }

        public void Add(IResult result)
        {
            IResult existing = FindItem(result.Text, result.Result.CursorKind);
            if(existing == null)
            {
                _items.Add(new Tuple<string, LibClang.CursorKind>(result.Text, result.Result.CursorKind), result);
                return;
            }
            if(existing != null && result.IsFunctionCall)
            {
                //existing.OverloadProvider.
            }
        }

        public IEnumerable<IResult> Results { get { return _items.Values; } }

        private IResult FindItem(string name, LibClang.CursorKind k)
        {
            IResult result;
            if(_items.TryGetValue(new Tuple<string, LibClang.CursorKind>(name, k), out result) == false)
                return null;
            return result;
        }
    }
}

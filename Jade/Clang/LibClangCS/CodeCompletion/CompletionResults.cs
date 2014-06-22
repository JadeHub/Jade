using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    public class Results
    {
        private List<Result> _results;
        
        internal unsafe Results(CodeCompletion.Library.CXCodeCompleteResults handle)
        {
            //we dont hold a ref to handle as it will be destroyed once the result set has been loaded
            _results = new List<Result>();

            for(uint i = 0;i < handle.NumberResults; i++)
            {
                Library.CXCompletionResult r = handle.Results[i];
                _results.Add(new Result(r));                
            }
        }

        public IEnumerable<Result> Items { get { return _results; } }
    }
}

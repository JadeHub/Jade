using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SymbolInspector
{
    public class NullSymbolViewModel : SymbolViewModelBase
    {
        public NullSymbolViewModel()
            :base(null)
        { }

        public override string DisplayText
        {
            get { return ""; }
        }
    }
}

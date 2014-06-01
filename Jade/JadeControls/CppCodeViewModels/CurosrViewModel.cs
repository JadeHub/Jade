using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClang;

namespace JadeControls.CppCodeViewModel
{
    public class CurosrViewModel : NotifyPropertyChanged
    {
        private Cursor _cursor;

        public CurosrViewModel(Cursor c)
        {
            _cursor = c;
        }
    }
}

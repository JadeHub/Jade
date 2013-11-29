using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.Symbols
{
    public class DeclViewModel : NotifyPropertyChanged
    {
        #region Data

        private CppView.IDeclaration _decl;

        public DeclViewModel(CppView.IDeclaration decl)
        {
            _decl = decl;
        }

        public string Name 
        {
            get { return _decl.Name; } 
        }

        public string Details
        {
            get 
            {
                if (_decl.Kind == LibClang.Indexer.EntityKind.Field)
                {
                    return string.Format("{0} is {1} at {2} {3}", Name, _decl.Type, _decl.Location.Path, _decl.Location); 
                }
                return string.Format("{0} at {1} {2}", Name, _decl.Location.Path, _decl.Location);
            }
        }

        public string Kind { get { return _decl.Kind.ToString(); } }

        public CppView.IDeclaration Declaration
        {
            get { return _decl; }
        }

        public override string ToString()
        {
            return _decl.ToString();
        }

        #endregion
    }
}

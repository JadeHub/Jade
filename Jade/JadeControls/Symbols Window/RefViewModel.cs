using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.Symbols
{
    public class RefViewModel
    {
        #region Data

        private CppView.IReference _ref;
        private CppView.IDeclaration _decl;

        public RefViewModel(CppView.IReference refr)
        {
            _ref = refr;
            _decl = _ref.ReferencedDecl;
        }

        public CppView.IReference Reference { get { return _ref; } }

        public string Name {get { return _ref.ToString(); } }

        public string Details
        {
            get
            {
                //return string.Format("{0} to {1} {2}", _ref, _decl.Location.Path, _decl.Location);
                return _ref.ToString();
                //return _ref.C
                
            }
        }

        public string Kind { get { return _decl == null ? "null" :  _decl.Kind.ToString(); } }

        public override string ToString()
        {
            return _ref.ToString();
        }
        
        #endregion
    }
}

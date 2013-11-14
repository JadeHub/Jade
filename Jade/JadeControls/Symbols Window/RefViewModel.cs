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

        public string Name {get { return _ref.ToString(); } }

        public string Details
        {
            get
            {
                return string.Format("{0} to {1} {2}", _ref, _decl.Location.File, _decl.Location);
                
            }
        }

        public string Kind { get { return _decl.Kind.ToString(); } }

        public override string ToString()
        {
            return _ref.ToString();
        }
        
        #endregion
    }
}

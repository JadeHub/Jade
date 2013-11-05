
namespace CppView
{
    public class Reference
    {
        #region Data

        private Declaration _referencedDecl;

        #endregion

        public Reference(Declaration referencedDecl)
        {
            _referencedDecl = referencedDecl;
        }    
    }
}

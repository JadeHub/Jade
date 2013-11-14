using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.Symbols
{
    public class SymbolsViewModel : NotifyPropertyChanged
    {
        #region Data

        private CppView.IProjectSymbolTable _index;

        private ObservableCollection<DeclViewModel> _decls;
        private ObservableCollection<RefViewModel> _refs;

        private DeclViewModel _selectedDecl;
        private RefViewModel _selectedRef;
        
        #endregion

        public SymbolsViewModel(JadeCore.Project.IProject project)
        {
            _index = project.SourceIndex.SymbolTable;
            _decls = new ObservableCollection<DeclViewModel>();
            _refs = new ObservableCollection<RefViewModel>();

            foreach (string declUsr in _index.DeclarationUSRs)
            {
                foreach (CppView.IDeclaration decl in _index.GetDeclarations(declUsr))
                    _decls.Add(new DeclViewModel(decl));
            }

            foreach (CppView.IReference r in _index.References)
            {
                _refs.Add(new RefViewModel(r));
            }
        }

        public ObservableCollection<DeclViewModel> Declarations
        {
            get { return _decls; }
        }

        public ObservableCollection<RefViewModel> References
        {
            get { return _refs; }
        }

        public DeclViewModel SelectedDeclaration
        {
            get { return _selectedDecl; }
            set
            {
                if (_selectedDecl != value)
                {
                    _selectedDecl = value;
                    OnPropertyChanged("SelectedDeclaration");
                }
            }
        }

        public RefViewModel SelectedReference
        {
            get { return _selectedRef; }
            set
            {
                if (_selectedRef != value)
                {
                    _selectedRef = value;
                    OnPropertyChanged("SelectedReference");
                }
            }
        }

        public void OnRefDoubleClick(RefViewModel refVm)
        {

        }

        public void OnDeclDoubleClick(DeclViewModel declVm)
        {

        }
    }
}

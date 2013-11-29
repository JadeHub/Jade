using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.Symbols
{
    using JadeUtils.IO;
    
    public class SymbolsWindowViewModel : NotifyPropertyChanged
    {
        #region Data

        private CppView.IProjectSymbolTable _symbolTable;
        private CppView.ISourceFileStore _fileStore;

        private ObservableCollection<FilePath> _files;
        private ObservableCollection<DeclViewModel> _decls;
        private ObservableCollection<RefViewModel> _refs;

        private DeclViewModel _selectedDecl;
        private RefViewModel _selectedRef;
        private FilePath _selectedFile;

        #endregion

        public SymbolsWindowViewModel(JadeCore.Project.IProject project)
        {
            _symbolTable = project.SourceIndex.SymbolTable;
            _fileStore = project.SourceIndex.FileStore;

            _files = new ObservableCollection<FilePath>();
            foreach (FilePath path in _fileStore.AllFiles)
            {
                _files.Add(path);
            }
            _decls = new ObservableCollection<DeclViewModel>();
            _refs = new ObservableCollection<RefViewModel>();

            if (_files.Count > 0)
                SelectedFile = _files[0];
        }

        private void PopulateFileSymbols(FilePath path)
        {
            _decls.Clear();
            _refs.Clear();

            CppView.ISymbolTable fileTable = _symbolTable;//.GetFileSymbolTable(path);
            if (fileTable != null)
            {
                foreach (string declUsr in fileTable.DeclarationUSRs)
                {
                    foreach (CppView.IDeclaration decl in fileTable.GetDeclarations(declUsr))
                        _decls.Add(new DeclViewModel(decl));
                }

                foreach (CppView.IReference r in fileTable.References)
                {
                    _refs.Add(new RefViewModel(r));
                }
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

        public ObservableCollection<FilePath> FileNames
        {
            get { return _files; }
        }

        public FilePath SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (_selectedFile != value)
                {
                    _selectedFile = value;
                    OnPropertyChanged("SelectedFileName");
                    PopulateFileSymbols(_selectedFile);
                }
            }
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
            CppView.IReference reference = refVm.Reference;
            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            cmdHandler.OnHighlightCodeLocation(reference.Location.Path, reference.Range.Start.Offset, reference.Range.End.Offset);
            cmdHandler.OnDisplayCodeLocation(refVm.Reference.Location);
        }

        public void OnDeclDoubleClick(DeclViewModel declVm, SymbolsWindow view)
        {            
            CppView.IDeclaration decl = declVm.Declaration;

            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            //cmdHandler.OnHighlightCodeLocation(decl.Location.File.Path, decl.Range.Start.Offset, decl.Range.End.Offset);
            CppView.ICodeRange range = new CppView.CodeRange(decl.LocalCursor, decl.Path);
            cmdHandler.OnHighlightCodeLocation(decl.Location.Path, range.Start.Offset, range.End.Offset);
            cmdHandler.OnDisplayCodeLocation(declVm.Declaration.Location);
        }
    }
}

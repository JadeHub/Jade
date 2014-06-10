using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeCore;

namespace JadeControls.SymbolInspector
{
    public abstract class SymbolViewModelBase : NotifyPropertyChanged
    {
        public SymbolViewModelBase(JadeCore.CppSymbols.ISymbolCursor symbol)
        {
            SymbolCursor = symbol;
        }

        public JadeCore.CppSymbols.ISymbolCursor SymbolCursor
        {
            get;
            private set;
        }

        public string Name
        {
            get { return SymbolCursor.Spelling; }
        }
    }
    /*
    public class MethodDeclarationViewModel : SymbolViewModelBase
    {
        public MethodDeclarationViewModel(JadeCore.CppSymbols.MethodDeclarationSymbol symbol)
            :base(symbol)
        {

        }

    }
    */
    public class ClassDeclarationViewModel : SymbolViewModelBase
    {
        private JadeCore.CppSymbols.ClassDeclarationSymbol _symbol;
        private SymbolGroupViewModel _constructorGroup;
        private SymbolGroupViewModel _methodGroup;
        private SymbolGroupViewModel _memberGroup;
        private SymbolGroupViewModel _baseClassGroup;

        public ClassDeclarationViewModel(JadeCore.CppSymbols.ClassDeclarationSymbol symbol)
            :base(symbol)
        {
            _symbol = symbol;
            _constructorGroup = new SymbolGroupViewModel("Constructors");
            _methodGroup = new SymbolGroupViewModel("Methods");
            _memberGroup = new SymbolGroupViewModel("Data Members");
            _baseClassGroup = new SymbolGroupViewModel("Base Classes");

            foreach (JadeCore.CppSymbols.ConstructorDeclarationSymbol ctor in symbol.Constructors)
            {
                _constructorGroup.AddSymbol(ctor);
            }

            foreach(JadeCore.CppSymbols.MethodDeclarationSymbol method in symbol.Methods)
            {
                _methodGroup.AddSymbol(method);
            }

            foreach(JadeCore.CppSymbols.ClassDeclarationSymbol b in symbol.BaseClasses)
            {
                _baseClassGroup.AddSymbol(b);
            }

        }

        public string TypeLabel
        {
            get { return "Class"; }
        }

        public SymbolGroupViewModel ConstructorGroup
        {
            get { return _constructorGroup; }
        }

        public SymbolGroupViewModel MethodGroup
        {
            get { return _methodGroup; }
        }

        public SymbolGroupViewModel DataMemberGroup
        {
            get { return _memberGroup; }
        }

        public SymbolGroupViewModel BaseClassGroup
        {
            get { return _baseClassGroup; }
        }
    }    
}

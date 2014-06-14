using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using JadeCore;

namespace JadeControls.SymbolInspector
{ 
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
                _constructorGroup.AddSymbol(new ConstructorViewModel(ctor));
            }

            foreach(JadeCore.CppSymbols.MethodDeclarationSymbol method in symbol.Methods)
            {
                _methodGroup.AddSymbol(new MethodDeclarationViewModel(method));
            }

            foreach(JadeCore.CppSymbols.ClassDeclarationSymbol b in symbol.BaseClasses)
            {
                _baseClassGroup.AddSymbol(new ClassDeclarationViewModel(b));
            }

            foreach (JadeCore.CppSymbols.DataMemberDeclarationSymbol data in symbol.DataMembers)
            {
                _memberGroup.AddSymbol(new DataMemberViewModel(data));
            }
        }

        public string TypeLabel
        {
            get 
            {
                return SymbolCursor.Cursor.Kind == LibClang.CursorKind.StructDecl ? "Struct" : "Class";
            }
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

        public override string DisplayText
        {
            get { return SourceText; }
        }
    }    
}

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
using CppCodeBrowser.Symbols;

namespace JadeControls.SymbolInspector
{ 
    public class ClassDeclarationViewModel : SymbolViewModelBase
    {
        private SymbolGroupViewModel _constructorGroup;
        private SymbolGroupViewModel _methodGroup;
        private SymbolGroupViewModel _memberGroup;
        private SymbolGroupViewModel _baseClassGroup;
        private ClassDecl _declaration;

        public ClassDeclarationViewModel(ClassDecl decl)
            :base(decl.Cursor)
        {
            _declaration = decl;
                        
            _constructorGroup = new SymbolGroupViewModel("Constructors");
            _methodGroup = new SymbolGroupViewModel("Methods");
            _memberGroup = new SymbolGroupViewModel("Data Members");
            _baseClassGroup = new SymbolGroupViewModel("Base Classes");

            foreach (var ctor in _declaration.Constructors)
            {
                _constructorGroup.AddSymbol(new ConstructorViewModel(ctor));
            }

            foreach (var method in from method in _declaration.Methods orderby method.Spelling select method)
            {
                _methodGroup.AddSymbol(new MethodDeclarationViewModel(method));
            }

            foreach (var b in _declaration.BaseClasses)
            {
                _baseClassGroup.AddSymbol(new ClassDeclarationViewModel(b));
            }

            foreach (var data in from member in _declaration.DataMembers orderby member.Spelling select member)
            {
                _memberGroup.AddSymbol(new DataMemberViewModel(data));
            }
        }

        public string TypeLabel
        {
            get 
            {
                return _declaration.IsStruct ? "Struct" : "Class";
                //return SymbolCursor.Cursor.Kind == LibClang.CursorKind.StructDecl ? "Struct" : "Class";
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
            get { return Spelling; }
        }
    }    
}

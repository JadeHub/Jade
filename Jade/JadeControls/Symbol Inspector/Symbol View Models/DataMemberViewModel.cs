using System;
using System.Text;
using System.Diagnostics;
using JadeCore.CppSymbols2;

namespace JadeControls.SymbolInspector
{
    public class DataMemberViewModel : SymbolViewModelBase
    {
        public DataMemberViewModel(DataMemberDeclarationSymbol symbol)
            : base(symbol)
        {

        }

        private JadeCore.CppSymbols2.DataMemberDeclarationSymbol CtorSymbol
        {
            get
            {
                Debug.Assert(SymbolCursor is DataMemberDeclarationSymbol);
                return SymbolCursor as DataMemberDeclarationSymbol;
            }
        }

        private string GetTypeString(LibClang.Type type)        
        {
            string result;

            string name = SymbolCursor.Spelling;

            if (type.PointeeType != null)
                result = GetTypeString(type.PointeeType);
            else if (type.DeclarationCursor != null)
                //If there is a declaration, use its name. This removes any namespaces from class / struct types. eg "ClassFoo" rather than "NamespaceBar::ClassFoo"
                result = type.DeclarationCursor.Spelling;
            else
                result = type.Spelling;
                        
            if (type.Kind == LibClang.TypeKind.Pointer)
                result = result + "*";

            if (type.Kind == LibClang.TypeKind.LValueReference)
                result = result + "&";

            if (type.Kind == LibClang.TypeKind.RValueReference)
                result = result + "&&";

            if (type.IsConstQualified)
                result = result + " const ";

            return result;
        }

        public override string DisplayText
        {
            get 
            {
                return "";// BuildDisplayText();
            }
        }

        public string TypeText
        {
            get { return GetTypeString(SymbolCursor.Cursor.Type); }
        }
    }
}

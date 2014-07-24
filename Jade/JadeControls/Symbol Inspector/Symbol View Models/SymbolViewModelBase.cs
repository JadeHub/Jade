using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;

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

        public string SourceText
        {
            get 
            { 
                return SymbolCursor != null ? SymbolCursor.SourceText : ""; 
            }
        }

        public string Spelling
        {
            get 
            {
                return SymbolCursor != null ? SymbolCursor.Spelling : "";
            }
        }

        public abstract string DisplayText
        {
            get;        
        }
    }
}
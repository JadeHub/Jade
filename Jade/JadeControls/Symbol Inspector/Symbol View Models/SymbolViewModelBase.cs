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
        public SymbolViewModelBase(LibClang.Cursor c)
        {
            Cursor = c;
        }

        public LibClang.Cursor Cursor
        {
            get;
            private set;
        }
        
        public string Spelling
        {
            get 
            {
                return Cursor != null ? Cursor.Spelling : "";
            }
        }

        public abstract string DisplayText
        {
            get;        
        }
    }
}
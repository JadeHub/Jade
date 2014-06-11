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
    public class BoldifySpellingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SymbolGroupItemViewModel item = value as SymbolGroupItemViewModel;
            string text = item.SourceText;
            string spelling = item.Spelling;
            TextBlock textBlock = new TextBlock();
            int index = text.IndexOf(spelling);
            if (index >= 0)
            {
                string before = text.Substring(0, index);
                string after = text.Substring(index + spelling.Length);
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(new Run() { Text = before });
                textBlock.Inlines.Add(new Run() { Text = spelling, FontWeight = FontWeights.Bold });
                textBlock.Inlines.Add(new Run() { Text = after });
            }
            else
            {
                textBlock.Text = text;
            }
            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

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

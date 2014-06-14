using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;

namespace JadeControls.SymbolInspector
{
    public class BoldifySpellingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SymbolViewModelBase item = value as SymbolViewModelBase;
            string text = item.DisplayText;
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
            textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
            return textBlock;
        }

        public object ConvertBack(object value1, Type targetType, object parameter, CultureInfo culture)
        {
            return value1;
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

        public string SourceText
        {
            get { return SymbolCursor.SourceText; }
        }

        public string Spelling
        {
            get { return SymbolCursor.Spelling; }
        }

        public abstract string DisplayText
        {
            get;        
        }
    }
}
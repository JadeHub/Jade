using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using CppCodeBrowser.Symbols;

namespace JadeControls.ContextTool  
{
    public class BoldifySpellingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DeclarationViewModel item = value as DeclarationViewModel;
            string text = item.Name;
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

    public class DeclarationViewModel : TreeItemBase, IComparable<DeclarationViewModel>
    {
        private IDeclaration _decl;

        public DeclarationViewModel(ITreeItem parent, IDeclaration decl)
            : base(parent, BuildName(decl))
        {
            _decl = decl;
        }

        public string Spelling { get { return _decl.Spelling; } }
        public string Usr { get { return _decl.Usr; } }
        public string KindString { get { return _decl.Kind.ToString(); } }

        private static string BuildName(IDeclaration decl)
        {
            if(decl is FunctionDeclBase)
            {
                return decl.Spelling + (decl as FunctionDeclBase).BuildParamText();
            }

            if(decl is ClassDecl)
            {
                ClassDecl c = decl as ClassDecl;

                
                if(c.TemplateKind != TemplateKind.NonTemplate)
                {
                    return decl.Cursor.DisplayName;
                }
            }

            return decl.Spelling;
        }

        public DeclarationViewModel FindOrAddChildDecl(IDeclaration child)
        {
            foreach(ITreeItem treeItem in Children)
            {
                if (treeItem is DeclarationViewModel && (treeItem as DeclarationViewModel).Usr == child.Usr)
                    return treeItem as DeclarationViewModel;
            }

            var r = new DeclarationViewModel(this, child);
            AddChild(r);
            return r;
        }

        public DeclarationViewModel Find(string usr)
        {
            if (usr == this.Usr)
                return this;
            foreach(var child in Children)
            {
                if(child is DeclarationViewModel)
                {
                    DeclarationViewModel result = (child as DeclarationViewModel).Find(usr);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private DeclarationViewModel GetChildAtIndex(int index)
        {
            if (Children[index] is DeclarationViewModel)
                return Children[index] as DeclarationViewModel;
            return null;
        }

        private void AddChild(DeclarationViewModel child)
        {
            bool added = false;
            for (int index = 0; index < Children.Count; index++)
            {
                DeclarationViewModel c = GetChildAtIndex(index);
                if (c == null) continue;
                
                if(c.CompareTo(child) > 0)
                {
                    added = true;
                    Children.Insert(index, child);
                    break;
                }
            }
            if (!added)
                Children.Add(child);
        }

        public int CompareTo(DeclarationViewModel other)
        {
            if(_decl.Kind != other._decl.Kind)
            {
                return _decl.Kind < other._decl.Kind ? -1 : 1;
            }

            if (Name != other.Name)
                return Name.CompareTo(other.Name);

            return Usr.CompareTo(other.Usr);
        }

        public DeclarationViewModel FindSelectedDeclaration()
        {
            ITreeItem sel = FindSelected();
            if (sel != null && sel is DeclarationViewModel)
                return sel as DeclarationViewModel;
            return null;
        }

        public void BrowseToLocation()
        {
            CppCodeBrowser.ICodeLocation loc = _decl.Location;
            if (loc == null) return;
            JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(loc, true, true));
        }
    }
}

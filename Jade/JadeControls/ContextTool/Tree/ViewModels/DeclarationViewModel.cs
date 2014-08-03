using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore.CppSymbols2;
using CppCodeBrowser.Symbols;

namespace JadeControls.ContextTool  
{
    public class DeclarationViewModel : TreeItemBase, IComparable<DeclarationViewModel>
    {
        private IDeclaration _decl;

        public DeclarationViewModel(ITreeItem parent, IDeclaration decl)
            : base(parent, BuildName(decl))
        {
            _decl = decl;
        }

        public string Usr { get { return _decl.Usr; } }
        public string KindString { get { return _decl.Kind.ToString(); } }

        private static string BuildName(IDeclaration decl)
        {
            if(decl is FunctionDeclBase)
            {
                return decl.Name + (decl as FunctionDeclBase).BuildParamText();
            }
            return decl.Name;
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
    }
}

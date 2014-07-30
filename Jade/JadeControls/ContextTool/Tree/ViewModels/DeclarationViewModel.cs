using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore.CppSymbols2;
using CppCodeBrowser.Symbols;

namespace JadeControls.ContextTool  
{
    public class DeclarationViewModel : TreeItemBase
    {
        private IDeclaration _decl;

        public DeclarationViewModel(ITreeItem parent, IDeclaration decl)
            :base(parent, decl.Name)
        {
            _decl = decl;
        }

        public string Usr { get { return _decl.Usr; } }

        public DeclarationViewModel FindOrAddChildDecl(IDeclaration child)
        {
            foreach(ITreeItem treeItem in Children)
            {
                if (treeItem is DeclarationViewModel && (treeItem as DeclarationViewModel).Usr == child.Usr)
                    return treeItem as DeclarationViewModel;
            }

            var r = new DeclarationViewModel(this, child);
            Children.Add(r);
            return r;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class TemplateClassDeclarationSymbol : ClassDeclarationSymbol
    {
      //  private List<MethodDeclarationSymbol> _templateParameters;

        public TemplateClassDeclarationSymbol(Cursor cur)
            : base(cur)
        {
        }

      /*  public IEnumerable<DataMemberDeclarationSymbol> DataMembers
        {
            get
            {
                if (_dataMembers == null)
                    _dataMembers = new List<DataMemberDeclarationSymbol>(GetType<DataMemberDeclarationSymbol>(LibClang.CursorKind.FieldDecl));
                return _dataMembers;
            }
        }*/
    }
}

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class ClassDecl : DeclarationBase
    {
//        private Dictionary<string, MethodDecl> _methods;

        public ClassDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {

  //          _methods = new Dictionary<string, MethodDecl>();
       /*     foreach(Cursor child in c.Children)
            {
                if(child.Kind == CursorKind.CXXMethod)
                {
                    MethodDecl m = FindOrAddMethod(child);
                }
            }*/
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Class; } }

      /*  private MethodDecl FindOrAddMethod(Cursor c)
        {
            MethodDecl method = null;
            if(_methods.TryGetValue(c.Usr, out method) == false)
            {
                Debug.Assert(c.IsDefinition || c.DefinitionCursor != null);

                if (c.IsDefinition)
                {
                    //If the method is declared inline c will be both the declartion and the definition. The definition Cursor of a method is the implemention.
                    method = new MethodDecl(c, c);
                }
                else
                {
                    //Otherwise the defintion and declaration are seperate
                    method = new MethodDecl(c.DefinitionCursor, c);
                }
                _methods.Add(c.Usr, method);
            }
            return method;
        }*/

        //methods ->definition, declaration (overrides?)
        //constructors
        //destructor
        //fields
        //bases
        //friends?


    }
}

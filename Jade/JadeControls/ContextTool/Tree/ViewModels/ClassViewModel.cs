using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore.CppSymbols;

namespace JadeControls.ContextTool
{/*
    public class ClassViewModel : TreeItemBase
    {
        public ClassViewModel(ITreeItem parent, ClassDeclarationSymbol symbol)
            :base(parent, symbol.Name)
        {
        }

        public override string TypeChar { get { return "C"; } }
    }
    */
    public class CursorViewModel : TreeItemBase
    {
        private string _typeChar;

        public CursorViewModel(ITreeItem parent, LibClang.Cursor c)
            : base(parent, c.Spelling)
        {
            _typeChar = MakeTypeChar(c);

            c.VisitChildren(delegate (LibClang.Cursor child, LibClang.Cursor p) 
            {
                if(Factory.CanMakeTreeItem(child))
                    Children.Add(Factory.MakeTreeItem(this, child));
                return LibClang.Cursor.ChildVisitResult.Continue;
            });
        }
/*
        private string MakeTypeChar(LibClang.Cursor c)
        {
            switch(c.Kind)
            {
                case(LibClang.CursorKind.ClassDecl):
                    return "C";
                case (LibClang.CursorKind.StructDecl):
                    return "S";
                case (LibClang.CursorKind.UnionDecl):
                    return "U";
                case (LibClang.CursorKind.EnumDecl):
                    return "E";
                case (LibClang.CursorKind.Namespace):
                    return "NS";
                case (LibClang.CursorKind.Constructor):
                    return "CT";
                case (LibClang.CursorKind.Destructor):
                    return "DT";
                case (LibClang.CursorKind.FieldDecl):
                    return "FD";
                case (LibClang.CursorKind.VarDecl):
                    return "VD";
                case (LibClang.CursorKind.FunctionDecl):
                    return "FC";
                case (LibClang.CursorKind.CXXMethod):
                    return "MD";
                case (LibClang.CursorKind.FunctionTemplate):
                    return "FT";
                case (LibClang.CursorKind.ClassTemplate):
                    return "CT";
                case (LibClang.CursorKind.ClassTemplatePartialSpecialization):
                    return "Spec";
            };
            return "";
        }*/

        private string MakeTypeChar(LibClang.Cursor c)
        {
            switch (c.Kind)
            {
                case (LibClang.CursorKind.ClassDecl):
                    return "Class";
                case (LibClang.CursorKind.StructDecl):
                    return "S";
                case (LibClang.CursorKind.UnionDecl):
                    return "U";
                case (LibClang.CursorKind.EnumDecl):
                    return "E";
                case (LibClang.CursorKind.Namespace):
                    return "Namespace";
                case (LibClang.CursorKind.Constructor):
                    return "Constructor";
                case (LibClang.CursorKind.Destructor):
                    return "DestructorT";
                case (LibClang.CursorKind.FieldDecl):
                    return "Field";
                case (LibClang.CursorKind.VarDecl):
                    return "Variable";
                case (LibClang.CursorKind.FunctionDecl):
                    return "Fuction";
                case (LibClang.CursorKind.CXXMethod):
                    return "Method";
                case (LibClang.CursorKind.FunctionTemplate):
                    return "FunctionTemplate";
                case (LibClang.CursorKind.ClassTemplate):
                    return "ClassTemplate";
                case (LibClang.CursorKind.ClassTemplatePartialSpecialization):
                    return "Spec";
            };
            return "";
        }

        public override string TypeChar { get { return _typeChar; } }
    }
}

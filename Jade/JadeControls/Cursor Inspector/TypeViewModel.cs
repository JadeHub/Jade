using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClang;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace JadeControls.CursorInspector
{
    public class TypeViewModel
    {
        private LibClang.Type _t;

        public TypeViewModel(LibClang.Type t)
        {
            _t = t;
        }

        [Category("Type"), Description("Blah.")]
        public string Spelling
        {
            get { return _t.Spelling; }
        }

        [Category("Type"), Description("Blah.")]
        public bool IsConstQualified
        {
            get { return _t.IsConstQualified; }
        }

        [Category("Type"), Description("Blah.")]
        public bool IsVolatileQualified
        {
            get { return _t.IsVolatileQualified; }
        }

        [Category("Type"), Description("Blah.")]
        public bool IsRestrictQualified
        {
            get { return _t.IsRestrictQualified; }
        }

        [Category("Type"), Description("Blah.")]
        public bool IsFunctionTypeVariadic
        {
            get { return _t.IsFunctionTypeVariadic; }
        }

        [Category("Type"), Description("Blah.")]
        public bool IsPOD
        {
            get { return _t.IsPOD; }
        }

        [Category("Type"), Description("Blah.")]
        public LibClang.Type.CallingConv CallingConvention
        {
            get { return _t.CallingConvention; }
        }

        [Category("Type"), Description("Blah.")]
        public int NumberOfArguments
        {
            get { return _t.NumberOfArguments; }
        }

        [Category("Type"), Description("Blah.")]
        public long NumberOfElements
        {
            get { return _t.NumberOfElements; }
        }

        [Category("Type"), Description("Blah.")]
        public Int64 ArraySize
        {
            get { return _t.ArraySize; }
        }

        [Category("Type"), Description("Blah.")]
        public Int64 Allignment
        {
            get { return _t.Alignment; }
        }

        [Category("Type"), Description("Blah.")]
        public Int64 SizeOf
        {
            get { return _t.SizeOf; }
        }

        [Category("Type"), Description("Blah.")]
        public LibClang.Type.RefQualifierKind RefQualifierKind
        {
            get { return _t.RefQualifier; }
        }

        [Category("Type"), Description("Blah.")]
        public LibClang.TypeKind Kind
        {
            get { return _t.Kind; }
        }

        private object ReturnType(LibClang.Type t)
        {
            if (t == null) return "null";
            return new TypeViewModel(t);
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object ElementType
        {
            get { return ReturnType(_t.ElementType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object ArrayElementType
        {
            get { return ReturnType(_t.ArrayElementType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object ClassType
        {
            get { return ReturnType(_t.ClassType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object Canonical
        {
            get { return ReturnType(_t.CanonicalType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object Pointee
        {
            get { return ReturnType(_t.PointeeType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public object Result
        {
            get { return ReturnType(_t.ResultType); }
        }

        [Category("Type"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel DeclarationCursor
        {
            get { return _t.DeclarationCursor != null ? new CursorViewModel(_t.DeclarationCursor) : null; }
        }
    }
}

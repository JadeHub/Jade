using LibClang;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace JadeControls.CursorInspector
{
    public class CursorViewModel
    {
        private Cursor Cursor
        {
            get;
            set;
        }

        public CursorViewModel(Cursor c)
        {
            Cursor = c;
        }

        public override string ToString()
        {
            return Spelling;
        }

        [Category("Cursor"), Description("Blah.")]
        public string DisplayName
        {
            get { return Cursor.DisplayName; }
        }

        [Category("Cursor"), Description("Blah.")]
        public string Spelling
        {
            get { return Cursor.Spelling; }
        }

        [Category("Cursor"), Description("Blah.")]
        public string USR
        {
            get { return Cursor.Usr; }
        }

        [Category("Cursor"), Description("Blah.")]
        public SourceRange Extent
        {
            get { return Cursor.Extent; }
        }

        [Category("Cursor"), Description("Blah.")]
        public LibClang.SourceLocation Location
        {
            get { return Cursor.Location; }
        }

        [Category("Cursor"), Description("Blah.")]
        public CursorKind Kind
        {
            get { return Cursor.Kind; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsDefinition
        {
            get { return Cursor.IsDefinition; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsReference
        {
            get { return Cursor.IsReference; }
        }

        [Category("Cursor"), Description("Blah.")]
        public LibClang.Cursor.AccessSpecifier AccessSpecifier
        {
            get { return Cursor.Access; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel SemanticParentCursor
        {
            get { return Cursor.SemanticParentCurosr != null ? new CursorViewModel(Cursor.SemanticParentCurosr) : null; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel LexicalParentCursor
        {
            get { return Cursor.LexicalParentCurosr != null ? new CursorViewModel(Cursor.LexicalParentCurosr) : null; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel ReferencedCursor
        {
            get { return Cursor.CursorReferenced != null ? new CursorViewModel(Cursor.CursorReferenced) : null; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel DefinitionCursor
        {
            get { return Cursor.DefinitionCursor != null ? new CursorViewModel(Cursor.DefinitionCursor) : null; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public object CanonicalCursor
        {
            get { return ReturnCursor(Cursor.CanonicalCursor); }
        }

        private object ReturnCursor(Cursor c)
        {
            if (c == null) return null;

            if (c == Cursor) return "this";

            return new CursorViewModel(c);
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public TypeViewModel Type
        {
            get { return new TypeViewModel(Cursor.Type); }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public TypeViewModel ResultType
        {
            get { return new TypeViewModel(Cursor.ResultType); }
        }

        [Category("Cursor"), Description("Blah.")]
        public TypeViewModel EnumIntegerType
        {
            get { return new TypeViewModel(Cursor.EnumIntegerType); }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsConstMethod
        {
            get { return Cursor.IsConstMethod; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsVirtualBase
        {
            get { return Cursor.IsVirtualBase; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsVirtual
        {
            get { return Cursor.IsVirtual; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsPureVirtual
        {
            get { return Cursor.IsPureVirtual; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsStatic
        {
            get { return Cursor.IsStatic; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsDynamicCall
        {
            get { return Cursor.IsDynamicCall; }
        }

        [Category("Cursor"), Description("Blah.")]
        public bool IsVariadic
        {
            get { return Cursor.IsVariadic; }
        }

        [Category("Cursor"), Description("Blah.")]
        public string IncludedFile
        {
            get { return Cursor.IncludedFile != null ? Cursor.IncludedFile.Name : string.Empty; }
        }

        [Category("Cursor"), Description("Blah.")]
        [ExpandableObject]
        public CursorViewModel TemplateSpecialisedCursorTemplate
        {
            get { return Cursor.TemplateSpecialisedCursorTemplate != null ? new CursorViewModel(Cursor.TemplateSpecialisedCursorTemplate) : null; }
        }

        [Category("Cursor"), Description("Blah.")]
        public LibClang.CursorKind TemplateCursorKind
        {
            get { return Cursor.TemplateCursorKind; }
        }

        /*
            OverriddenCursors
            Argument cursors
         */

        #region Type Information

        #endregion
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * 
clang_getTypedefDeclUnderlyingType
clang_getEnumDeclIntegerType
clang_getEnumConstantDeclValue
clang_getEnumConstantDeclUnsignedValue
clang_getFieldDeclBitWidth
clang_Cursor_isBitField

clang_Cursor_getCommentRange
clang_Cursor_getRawCommentText
clang_Cursor_getBriefCommentText
clang_Cursor_getParsedComment

clang_getCursorReferenceNameRange
 * */

namespace LibClang
{    
    /// <summary>
    /// An immutable wrapper around libclang's Cursor type.
    /// A Cursor represents an element in the AST of a translation unit.
    /// 
    /// Property comments taken from libclang.h.
    /// see http://clang.llvm.org/doxygen/Index_8h.html for more details.
    /// </summary>
    public sealed class Cursor
    {
        #region Inner classes 

        private sealed class OverriddenCursorSet
        {
            internal static OverriddenCursorSet Empty;

            unsafe static OverriddenCursorSet()
            {
                Empty = new OverriddenCursorSet(null, 0, null);
            }

            internal unsafe static OverriddenCursorSet CreateOverriddenCursorSet(Cursor c, ITranslationUnitItemFactory factory)
            {
                Library.CXCursor* overrides;
                uint count;
                Library.clang_getOverriddenCursors(c.Handle, &overrides, &count);
                if (count == 0) return Empty;
                OverriddenCursorSet result = new OverriddenCursorSet(overrides, count, factory);
                //Assuming that this only deletes the array allocated above and that the handles remain valid
                Library.clang_disposeOverriddenCursors(overrides);
                return result;
            }

            private List<Cursor> _cursors;

            private unsafe OverriddenCursorSet(Library.CXCursor* handles, uint count, ITranslationUnitItemFactory factory)
            {
                _cursors = new List<Cursor>();
                for (uint i = 0; i < count; i++)
                {
                    _cursors.Add(factory.CreateCursor(handles[i]));
                }
            }

            public IEnumerable<Cursor> Cursors { get { return _cursors; } }
        }

        #endregion

        public enum AccessSpecifier
        {
            Invalid = 0,
            Public = 1,
            Protected = 2,
            Private = 3
        }

        internal delegate Cursor CreateCursorDel(Library.CXCursor handle);

        #region Data

        private readonly TranslationUnit _translationUnit;
        private readonly ITranslationUnitItemFactory _itemFactory;
        private SourceLocation _location;
        private SourceRange _extent;
        private string _spelling;
        private readonly Type _type;
        private string _usr;
        private List<Cursor> _argumentCursors;
        private Type _resultType;
        private string _displayName;
        private OverriddenCursorSet _overriddenCursors;
        private File _includedFile;
        private Type _enumIntType;
        private Cursor _templateSpecialisedCursorTemplate;

        #endregion

        #region Construction

        /// <summary>
        /// Create a new Cursor object.
        /// </summary>
        /// <param name="handle">Handle to a non null cursor obect.</param>
        /// <param name="itemFactory">TranslationUnit's item factory / item cache.</param>
        internal Cursor(Library.CXCursor handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(!handle.IsNull);
            Debug.Assert(handle.IsValid);
            Handle = handle;
            _itemFactory = itemFactory;
            _translationUnit = _itemFactory.TranslationUnit;
            Kind = Library.clang_getCursorKind(Handle);
            Library.CXType typeHandle = Library.clang_getCursorType(Handle);
            if (typeHandle.IsValid)
                _type = _itemFactory.CreateType(typeHandle);
        }
                
        #endregion

        #region Properties

        public TranslationUnit TranslationUnit
        {
            get { return _translationUnit; }
        }

        /// <summary>
        /// SourceRange object representing the extent of this cursor.
        /// 
        /// Retrieve the physical extent of the source construct referenced by
        /// the given cursor.
        ///
        /// The extent of a cursor starts with the file/line/column pointing at the
        /// first character within the source construct that the cursor refers to and
        /// ends with the last character withinin that source construct. For a
        /// declaration, the extent covers the declaration itself. For a reference,
        /// the extent covers the location of the reference (e.g., where the referenced
        /// entity was actually used).
        /// </summary>
        public SourceRange Extent
        {
            get { return _extent ?? (_extent = MakeExtent()); }
        }

        private SourceRange MakeExtent()
        {
            Library.SourceRange range = Library.clang_getCursorExtent(Handle);
            if (range.IsNull)
                return null;
            return _itemFactory.CreateSourceRange(Library.clang_getCursorExtent(Handle));
        }

        internal Library.CXCursor Handle
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Return the Kind of this Cursor.
        /// </summary>
        public CursorKind Kind
        {
            get;
            private set;
        }
                
        public string Spelling
        {
            get { return _spelling ?? (_spelling = Library.clang_getCursorSpelling(Handle).ManagedString); }
        }        

        /// <summary>
        /// A SourceLocation object representing the cursor's location.
        /// 
        /// Retrieve the physical location of the source construct referenced
        ///  by the given cursor.
        ///
        /// The location of a declaration is typically the location of the name of that
        /// declaration, where the name of that declaration would occur if it is
        /// unnamed, or some keyword that introduces that particular declaration.
        /// The location of a reference is where that reference occurs within the
        /// source code.
        /// </summary>
        public SourceLocation Location
        {
            get
            {
                if (_location == null)
                {
                    Library.SourceLocation loc = Library.clang_getCursorLocation(Handle);
                    if (loc.IsNull == false)
                        _location = _itemFactory.CreateSourceLocation(loc);
                }
                return _location;
            }
        }

        /// <summary>
        /// Return the Type object associated wiht this cursor.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Return a Unified Symbol Resolution (USR) for the entity referenced
        /// by the given cursor.
        ///
        /// A Unified Symbol Resolution (USR) is a string that identifies a particular
        /// entity (function, class, variable, etc.) within a program. USRs can be
        /// compared across translation units to determine, e.g., when references in
        /// one translation refer to an entity defined in another translation unit.
        /// </summary>
        public string Usr
        {
            get { return _usr ?? (_usr = Library.clang_getCursorUSR(Handle).ManagedString);}        
        }

        /// <summary>
        /// Returns the canonical cursor, if any, for the entity referred to by the given cursor.
        /// May return 'this'.
        /// </summary>
        public Cursor CanonicalCursor
        {
            get 
            {
                Library.CXCursor cur = Library.clang_getCursorDefinition(Handle);
                if (cur == Handle)
                    return this;
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns true if the declaration pointed to by this cursor
        /// is also a definition of that entity.
        /// </summary>
        public bool IsDefinition
        {
            get { return Library.clang_isCursorDefinition(Handle) != 0; }
        }

        /// <summary>
        /// For a cursor that is either a reference to or a declaration
        /// of some entity, return a cursor that describes the definition of
        /// that entity. 
        /// </summary>
        public Cursor DefinitionCursor
        {
            get 
            {
                Library.CXCursor cur = Library.clang_getCursorDefinition(Handle);
                return (cur.IsNull || cur == Handle) ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns true if the given cursor kind represents a simple reference.
        /// </summary>
        public bool IsReference
        {
            get { return Library.clang_isReference(Kind) != 0; }
        }
        
        /// <summary>
        /// For a cursor that is a reference, retrieve a cursor representing the
        /// entity that it references.
        /// </summary>
        public Cursor CursorReferenced
        {
            get
            {
                Library.CXCursor cur = Library.clang_getCursorReferenced(Handle);
                return (cur.IsNull || cur == Handle) ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns the lexical parent of the given cursor.
        /// </summary>
        public Cursor LexicalParentCurosr
        {
            get
            {
                Library.CXCursor cur = Library.clang_getCursorLexicalParent(Handle);
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns the semantic parent of the given cursor.
        /// </summary>
        public Cursor SemanticParentCurosr
        {
            get
            {
                Library.CXCursor cur = Library.clang_getCursorSemanticParent(Handle);
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        public AccessSpecifier Access
        {
            get { return (AccessSpecifier)Library.clang_getCXXAccessSpecifier(Handle); }
        }
        
        public Type ResultType
        {
            get 
            { 
                if(_resultType == null)
                {
                    Library.CXType type = Library.clang_getCursorResultType(Handle);
                    if (type != null && type.IsValid)
                        _resultType = _itemFactory.CreateType(type);
                }
                return _resultType;
            }
        }

        public string DisplayName
        {
            get { return _displayName ?? (_displayName = Library.clang_getCursorDisplayName(Handle).ManagedString); }
        }

        public IEnumerable<Cursor> ArgumentCursors
        {
            get
            {
                if(_argumentCursors == null)
                {
                    _argumentCursors = new List<Cursor>();
                    int t = Library.clang_Cursor_getNumArguments(Handle);
                    for(int i = 0; i < Library.clang_Cursor_getNumArguments(Handle); i++)
                    {
                        _argumentCursors.Add(_itemFactory.CreateCursor(Library.clang_Cursor_getArgument(Handle, (uint)i)));
                    }
                }
                return _argumentCursors;
            }
        }

        private static void ParseMethodUSR(string usr, out bool isConst)
        {
            int hashLoc = usr.LastIndexOf("#");
            if (hashLoc == -1 || hashLoc == usr.Length - 1)
            {
                isConst = false;
                return;
            }
            hashLoc++;
            int val = usr[hashLoc];

            isConst = (val & 0x1) == 0x1;
        }

        public bool IsConstMethod
        {
            get 
            {
                bool c;

                ParseMethodUSR(Usr, out c);

                return c; 
            }
        }

        /// <summary>
        /// Returns true if the cursor is of kind CX_CXXBaseSpecifier and is a virtual base
        /// </summary>
        public bool IsVirtualBase
        {
            get { return Library.clang_isVirtualBase(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the cursor is of kind CXXMethod and is a virtual method
        /// </summary>
        public bool IsVirtual
        {
            get { return Library.clang_CXXMethod_isVirtual(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the cursor is of kind CXXMethod and is a pure virtual method
        /// </summary>
        public bool IsPureVirtual
        {
            get { return Library.clang_CXXMethod_isPureVirtual(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the cursor is of kind CXXMethod and is a static method
        /// </summary>
        public bool IsStatic
        {
            get { return Library.clang_CXXMethod_isStatic(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the cursor is a c++ method call and the method is virtual.
        /// </summary>
        public bool IsDynamicCall
        {
            get { return Library.clang_Cursor_isDynamicCall(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the cursor is a variadic method or class
        /// </summary>
        public bool IsVariadic
        {
            get { return Library.clang_Cursor_isVariadic(Handle) != 0; }
        }

        /// <summary>
        /// Retrieve the integer type of an enum declaration.
        /// </summary>
        public Type EnumIntegerType
        {
            get
            {
                if(_enumIntType == null)
                {
                    Library.CXType t = Library.clang_getEnumDeclIntegerType(Handle);
                    if(t != null && t.IsValid)
                        _enumIntType = _itemFactory.CreateType(t);
                }
                return _enumIntType;
            }
        }

        /// <summary>
        /// For method cursors this is the list of methods they override.
        /// Multiple inheritance can result in a method overriding multiple methods
        /// Only methods overridden from immediate bases are returned. To discover all overridden methods in the case of multiple inheritance use this recursively
        /// </summary>
        public IEnumerable<Cursor> OverriddenCursors
        {
            get 
            {
                if (_overriddenCursors == null && Kind == CursorKind.CXXMethod)
                {
                    _overriddenCursors = OverriddenCursorSet.CreateOverriddenCursorSet(this, _itemFactory);
                }
                return _overriddenCursors == null ? Enumerable.Empty<Cursor>() : _overriddenCursors.Cursors; 
            }
        }
        
        /// <summary>
        /// Returns the included file for Cursors of type InclusionDirective or null
        /// </summary>
        /// <returns></returns>        
        public File IncludedFile
        {
            get 
            { 
                if(_includedFile == null)
                {
                    IntPtr fileHandle = Library.clang_getIncludedFile(Handle);
                    if (fileHandle != IntPtr.Zero)
                        _includedFile = _itemFactory.CreateFile(fileHandle);
                }
                return _includedFile;
            }
        }

        /// <summary>
        /// Given a cursor that may represent a specialization or instantiation
        /// of a template, retrieve the cursor that represents the template that it
        /// specializes or from which it was instantiated.
        /// </summary>
        public Cursor TemplateSpecialisedCursorTemplate
        {
            get
            {
                if(_templateSpecialisedCursorTemplate == null)
                {
                    Library.CXCursor c = Library.clang_getSpecializedCursorTemplate(Handle);
                    if (c != null && c.IsValid)
                        _templateSpecialisedCursorTemplate = _itemFactory.CreateCursor(c);
                }
                return _templateSpecialisedCursorTemplate;
            }
        }

        /// <summary>
        /// Given a cursor that represents a template, determine
        /// the cursor kind of the specializations would be generated by instantiating
        /// the template.
        /// 
        /// This routine can be used to determine what flavor of function template,
        /// class template, or class template partial specialization is stored in the
        /// cursor. For example, it can describe whether a class template cursor is
        /// declared with "struct", "class" or "union".
        /// </summary>
        public CursorKind TemplateCursorKind
        {
            get { return Library.clang_getTemplateCursorKind(Handle); }
        }

        /// <summary>
        /// Determine the number of overloaded declarations referenced by a CXCursor_OverloadedDeclRef cursor.
        /// </summary>
        public uint OverloadedDeclarationCount
        {
            get { return Library.clang_getNumOverloadedDecls(Handle); }
        }

        public Cursor GetOverloadedDeclaration(uint index)
        {
            if (index < OverloadedDeclarationCount)
            {
                Library.CXCursor cur = Library.clang_getOverloadedDecl(Handle, index);
                if (cur != null && cur.IsValid)
                    return _itemFactory.CreateCursor(cur);
            }
            return null;
        }
        
        /// <summary>
        /// Returns true if the cursor is the null cursor or the cursor kind represents an invalid cursor.
        /// </summary>
        public bool Valid { get { return Handle.IsValid; } }

        #endregion

        #region Child visitation
        
        /// <summary>
        /// Value returned by the visitor callback to control the ast navigation.
        /// </summary>
        public enum ChildVisitResult
        {
            Break,
            Continue,
            Recurse,
        };

        /// <summary>
        /// Cursor visitor callback
        /// </summary>
        /// <param name="cursor">Cursor being visited.</param>
        /// <param name="parent">Parent of cursor being visited.</param>
        /// <returns>ChildVisitResult</returns>
        public delegate ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent);

        /// <summary>
        /// Inspect cursor's children via a callback.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitChildren(CursorVisitor visitor)
        {
            Library.clang_visitChildren(
                Handle,
                (cursor, parent, data) => visitor(_itemFactory.CreateCursor(cursor), _itemFactory.CreateCursor(parent)),
                IntPtr.Zero);
        }

        public IList<Cursor> Children
        {
            get
            {
                IList<Cursor> children = new List<Cursor>();

                VisitChildren(delegate(Cursor cursor, Cursor parent)
                {
                    children.Add(cursor);
                    return ChildVisitResult.Continue;
                });
                return children;
            }
        }

        #endregion

        #region Object overrides

        public override string ToString()
        {
            return Kind + " at " + Location.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Cursor)
            {
                return (obj as Cursor).Handle == Handle;
            }
            return false;
        }

        private int hash = 0;

        public override int GetHashCode()
        {
            return hash == 0 ? (hash = Handle.GetHashCode()) : hash;
        }

        #endregion

        #region Static operator functions

        public static bool operator == (Cursor left, Cursor right)
        {
            if ((object)left == null && (object)right == null) return true;
            if ((object)left == null || (object)right == null) return false;
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return !(left == right);
        }

        #endregion
    }
}

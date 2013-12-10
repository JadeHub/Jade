﻿using System;
using System.Diagnostics;

namespace LibClang
{
    public class Cursor
    {
        #region Data

        //private
        private SourceRange _extent;
        private string _spelling;
        private Type _type;
        private string _usr;

        #endregion

        #region Properties

        private SourceRange BuildExtent()
        {
            return new SourceRange(Dll.clang_getCursorExtent(Handle));
        }

        public SourceRange Extent
        {
            get { return _extent ?? (_extent = BuildExtent()); }
        }

        #endregion

        internal Dll.Cursor Handle
        {
            get;
            private set;
        }

        internal Cursor(Dll.Cursor handle)
        {
            Debug.Assert(handle.IsNull() == false);
            Handle = handle;
            Kind = Dll.clang_getCursorKind(Handle);            
        }

        public CursorKind Kind
        {
            get;
            private set;
        }
                
        public string Spelling
        {
            get { return _spelling ?? (_spelling = Dll.clang_getCursorSpelling(Handle).ManagedString); }
        }

        public SourceLocation Location
        {
            get
            {
                return new SourceLocation(Dll.clang_getCursorLocation(Handle));
            }
        }

        public Type Type
        {
            get { return _type ?? (_type = new Type(Dll.clang_getCursorType(Handle))); }
        }

        public string Usr
        {
            get { return _usr ?? (_usr = Dll.clang_getCursorUSR(Handle).ManagedString);}
        
        }

        public Cursor CanonicalCursor
        {
            get 
            {
                Dll.Cursor cur = Dll.clang_getCursorDefinition(Handle);
                if (cur == Handle)
                    return this;
                return cur.IsNull() ? null : new Cursor(cur);
            }
        }

        public bool IsDefinition
        {
            get { return Dll.clang_isCursorDefinition(Handle) != 0; }
        }

        public Cursor Definition
        {
            get 
            {
                Dll.Cursor cur = Dll.clang_getCursorDefinition(Handle);
                return (cur.IsNull() || cur == Handle) ? null : new Cursor(cur);
            }
        }

        public bool IsReference
        {
            get { return Dll.clang_isReference(Kind) != 0; }
        }
                
        public Cursor CursorReferenced
        {
            get
            {
                Dll.Cursor cur = Dll.clang_getCursorReferenced(Handle);
                return (cur.IsNull() || cur == Handle) ? null : new Cursor(cur);
            }
        }

        public Cursor LexicalParentCurosr
        {
            get
            {
                Dll.Cursor cur = Dll.clang_getCursorLexicalParent(Handle);
                return cur.IsNull() ? null : new Cursor(cur);
            }
        }

        public Cursor SemanticParentCurosr
        {
            get
            {
                Dll.Cursor cur = Dll.clang_getCursorSemanticParent(Handle);
                return cur.IsNull() ? null : new Cursor(cur);
            }
        }

        public bool IsInvalid { get { return Dll.clang_isInvalid(Kind) != 0; } }

        public enum ChildVisitResult
        {
            Break,
            Continue,
            Recurse,
        };

        public delegate ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent);

        public void VisitChildren(CursorVisitor visitor)
        {
            Dll.clang_visitChildren(
                Handle,
                (cursor, parent, data) => visitor(new Cursor(cursor), new Cursor(parent)),
                IntPtr.Zero);
        }

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

        public override string ToString()
        {
            return Kind + " in " + Location.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Cursor)
            {
                return (obj as Cursor).Handle == Handle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}

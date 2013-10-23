using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public class Cursor
    {
        internal Dll.Cursor Handle
        {
            get;
            private set;
        }

        internal Cursor(Dll.Cursor handle)
        {
            Handle = handle;
            Kind = Dll.clang_getCursorKind(Handle);
        }

        public CursorKind Kind
        {
            get;
            private set;
        }

        private string _spelling;
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

        public override string ToString()
        {
            return Spelling + " is " + Kind + " in ";// +Location;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public enum EntityKind
    {
        Namespace,
        Class,
        Struct,
        Union,
        Function,
        FunctionArg,
        Variable,
        Enum,
        EnumConstant,
        Constructor,
        Destructor,
        Method,
        Field,        
        Typedef
    };

    public interface IDeclaration : ISymbol
    {
        EntityKind Kind { get; }
        string Usr { get; }        
    }

    public interface IHasDefinition
    {
        void UpdateDefinition(Cursor c);
        Cursor DefinitionCursor { get; }
        bool HasDefinitionCursor { get; }
    }

    public abstract class DeclarationBase : IDeclaration
    {
        private Cursor _cursor;
        private ICodeLocation _location;
        private ISymbolTable _table;

        protected DeclarationBase(Cursor c, ISymbolTable table)
        {
            _cursor = c;
            _table = table;
            _location = new CodeLocation(c.Location); //todo - replace for doc tracking
        }

        public Cursor Cursor { get { return _cursor; } }
        public ICodeLocation Location { get { return _location; } }
        public int SpellingLength { get { return _cursor.Spelling.Length; } }
        public string Usr { get{ return _cursor.Usr;}}
        public string Spelling { get { return Cursor.Spelling; } }

        public abstract string Name { get ;}
        public abstract EntityKind Kind { get; }

        protected ISymbolTable Table { get { return _table; } }

        public override string ToString()
        {
            return string.Format("Definition of {0} {1} at {2}", Kind, Name, Location);
        }

        public override bool Equals(object obj)
        {
            if (obj is IDeclaration)
                return Usr == (obj as IDeclaration).Usr;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Usr.GetHashCode();
        }
    }
}
using System;
using System.Collections.Generic;

namespace LibClang
{
    /// <summary>
    /// A Collection of WrapperType objects keyed by HandleType. Used by TranslationUnit to cache objects of 
    /// type Cursor, SourceLocation etc.
    /// </summary>
    /// <typeparam name="HandleType"></typeparam>
    /// <typeparam name="WrapperType"></typeparam>
    public interface IWrapperObjectStore <HandleType, WrapperType> where WrapperType : class
    {
        bool Contains(HandleType handle);
        WrapperType FindOrAdd(HandleType handle);        
        void Clear();
    }

    /// <summary>
    /// Abstract base type for collection of WrapperType objects. Creation of WrapperType objects from HandleTYpe is defered to derived classes.
    /// </summary>
    /// <typeparam name="HandleType"></typeparam>
    /// <typeparam name="WrapperType"></typeparam>
    public abstract class WrapperObjectStore<HandleType, WrapperType> : IWrapperObjectStore<HandleType, WrapperType> where WrapperType : class
    {
        private Dictionary<HandleType, WrapperType> _items;

        public WrapperObjectStore()
        {
            _items = new Dictionary<HandleType, WrapperType>();
        }

        public bool Contains(HandleType handle)
        {
            return _items.ContainsKey(handle);
        }

        public WrapperType FindOrAdd(HandleType handle)
        {
            WrapperType wrapper;

            if (_items.TryGetValue(handle, out wrapper) == false)
            {
                wrapper = Create(handle);
                _items.Add(handle, wrapper);
            }
            return wrapper;
        }

        public void Clear()
        {
            _items.Clear();
        }

        protected abstract WrapperType Create(HandleType h);
    }

    /// <summary>
    /// Cache of Cursor objects.
    /// </summary>
    internal class CursorStore : WrapperObjectStore<Library.CXCursor, Cursor> 
    {
        private ITranslationUnitItemFactory _itemFactory;

        public CursorStore(ITranslationUnitItemFactory itemFactory) 
        {
            _itemFactory = itemFactory;            
        }

        protected override Cursor Create(Library.CXCursor handle)
        {
            if (handle == Library.CXCursor.NullCursor)
                throw new ArgumentException("Cursor Handle is a NullCursor");
            return new Cursor(handle, _itemFactory);
        }
    }

    /// <summary>
    /// Cache of File objects.
    /// </summary>
    internal class FileStore : WrapperObjectStore<IntPtr, File> 
    {
        private ITranslationUnitItemFactory _itemFactory;

        public FileStore(ITranslationUnitItemFactory itemFactory) 
        {
            _itemFactory = itemFactory;
        }

        protected override File Create(IntPtr handle)
        {            
            if(handle == IntPtr.Zero)
                throw new ArgumentException("File Handle is null");
            return new File(handle, _itemFactory);
        }
    }

    /// <summary>
    /// Cache of SourceLocation objects.
    /// </summary>
    internal class SourceLocationStore : WrapperObjectStore<Library.SourceLocation, SourceLocation>
    {
        private ITranslationUnitItemFactory _itemFactory;

        public SourceLocationStore(ITranslationUnitItemFactory itemFactory) 
        {
            _itemFactory = itemFactory;
        }

        protected override SourceLocation Create(Library.SourceLocation handle)
        {
            if (handle.IsNull)
                throw new ArgumentException("SourceLocation Handle is null");
            return new SourceLocation(handle, _itemFactory);
        }
    }

    /// <summary>
    /// Cache of SourceRange objects.
    /// </summary>
    internal class SourceRangeStore : WrapperObjectStore<Library.CXSourceRange, SourceRange>
    {
        private ITranslationUnitItemFactory _itemFactory;

        public SourceRangeStore(ITranslationUnitItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
        }

        protected override SourceRange Create(Library.CXSourceRange handle)
        {
            if(handle.IsNull)
                throw new ArgumentException("SourceRange Handle is null");
            return new SourceRange(handle, _itemFactory);
        }
    }

    /// <summary>
    /// Cache of Type objects.
    /// </summary>
    internal class TypeStore : WrapperObjectStore<Library.CXType, Type>
    {
        private ITranslationUnitItemFactory _itemFactory;

        public TypeStore(ITranslationUnitItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
        }

        protected override Type Create(Library.CXType handle)
        {
            if(!handle.IsValid)
                throw new ArgumentException("CXType Handle is Invalid");
            return new Type(handle, _itemFactory);
        }
    }

    /// <summary>
    /// Cache of Diagnostic objects.
    /// </summary>
    internal class DiagnosticStore : WrapperObjectStore<IntPtr, Diagnostic>
    {
        private ITranslationUnitItemFactory _itemFactory;

        public DiagnosticStore(ITranslationUnitItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
        }

        protected override Diagnostic Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException("Diagnostic Handle is null");
            return new Diagnostic(handle, _itemFactory);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Output
{
    public enum Level
    {
        Info,
        Warn,
        Err,
        Crit
    }
    
    public enum Source
    {
        JadeDebug,
        Compilation,
        Executiom
    }
    
    public interface IItem
    {
        UInt64 Id { get; }
        Level Level { get; }
        Source Source { get; }
        string Message { get; }
        DateTime Time { get; }

        void Destroy();

    }

    public class Item : IItem
    {
        #region Data

        private IOutputController _controller;
        private readonly UInt64 _id;
        
        #endregion

        #region Constructor

        public Item(IOutputController ic, UInt64 id, Source s, Level l, string message)
        {
            _controller = ic;
            _id = id;
            Source = s;
            Level = l;
            Message = message;
            Time = DateTime.Now;
        }

        #endregion

        public override int GetHashCode()
        {
            return (int)_id;
        }

        public override bool Equals(object obj)
        {
            return Id == ((IItem)obj).Id;
        }

        #region Properties

        public UInt64 Id { get { return _id; } }

        public Level Level
        {
            get;
            set;
        }

        public Source Source 
        {
            get;
            set;
        }

        public string Message 
        { 
            get;
            set;
        }

        public DateTime Time
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods
        
        public void Destroy()
        {

        }

        #endregion
    }

    public interface IOutputController
    {
        ObservableCollection<IItem> Items { get; }
        IItem Create(Source source, Level level, string message);
        void Clear();
    }

    public class OutputController : IOutputController
    {
        private static UInt64 _nextId;
        private ObservableCollection<IItem> _Items;

        public OutputController()
        {
            _Items = new ObservableCollection<IItem>();
        }

        public IItem Create(Source source, Level level, string message)
        {
            IItem item = new Item(this, ++_nextId, source, level, message);
            _Items.Add(item);
            return item;
        }

        public ObservableCollection<IItem> Items 
        {
            get { return _Items; } 
        }

        public void Clear()
        {
            _Items.Clear();
        }
    }
}

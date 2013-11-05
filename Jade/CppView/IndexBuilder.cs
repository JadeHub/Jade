
namespace CppView
{
    public enum IndexBuilderState
    {
        Running,
        Suspended,
        Stopped
    }

    public enum IndexBuilderItemPriority
    {
        Immediate,
        High,
        Low
    }

    public interface IIndexBuilder
    {
        void Start();
        void Stop();

        IndexBuilderState State { get; }

        void AddFile(JadeUtils.IO.IFileHandle file, IndexBuilderItemPriority priority);
        void RemoveItem(JadeUtils.IO.IFileHandle file);
    }

    public class IndexBuilder : IIndexBuilder
    {
        #region Data

        private IndexBuilderState _state;
        private IProjectIndex _index;

        #endregion

        public IndexBuilder(IProjectIndex index)
        {
            _index = index;
            _state = IndexBuilderState.Stopped;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public IndexBuilderState State
        {
            get { return _state; }
        }

        public void AddFile(JadeUtils.IO.IFileHandle file, IndexBuilderItemPriority priority)
        {

        }

        public void RemoveItem(JadeUtils.IO.IFileHandle file)
        {

        }
    }
}

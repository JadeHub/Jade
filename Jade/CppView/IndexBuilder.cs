
namespace CppView
{
    public enum IndexBuilderState
    {
        Running,
        Suspended,
        Stopped
    }

    public interface IIndexBuilder
    {
        void Start();
        void Stop();

        IndexBuilderState State { get; }
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

            //watch the files in project, push them through a LibClang.Indexer updating _index as we go                                                                                                                                                             
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
    }
}

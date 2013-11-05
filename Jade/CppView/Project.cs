
namespace CppView
{
    public class Project
    {
        #region Data

        private IIndexBuilder _indexBuilder;

        #endregion

        public Project(IIndexBuilder indexBuilder)
        {
            _indexBuilder = indexBuilder;
        }

        //maintain a database of the source indexes by feeding sourcefiles through a multi threaded indexer
        //allow multiple observers to watch the database
    }
}

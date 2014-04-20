using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Search
{
    public interface ISearchController
    {
        void RegisterSearch(ISearch search);
        void RemoveSearch(ISearch search);

        ReadOnlyObservableCollection<ISearch> Searches { get; }
        ISearch Current { get; set; }
    }
}

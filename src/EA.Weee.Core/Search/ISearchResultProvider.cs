namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISearchResultProvider<T> where T : SearchResult
    {
        Task<IList<T>> FetchAll();
    }
}

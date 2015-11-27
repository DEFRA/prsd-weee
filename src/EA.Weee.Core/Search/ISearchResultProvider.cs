namespace EA.Weee.Core.Search
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearchResultProvider<T> where T : SearchResult
    {
        Task<IList<T>> FetchAll();
    }
}

namespace EA.Weee.Core.Search
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearcher<T> where T : SearchResult
    {
        Task<IList<T>> Search(string searchTerm, int maxResults, bool asYouType);
    }
}

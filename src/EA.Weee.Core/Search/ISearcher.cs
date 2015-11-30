namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISearcher<T> where T : SearchResult
    {
        Task<IList<T>> Search(string searchTerm, int maxResults, bool asYouType);
    }
}

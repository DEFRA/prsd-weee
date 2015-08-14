namespace EA.Weee.Web.Infrastructure.Paging
{
    using System.Collections.Generic;

    public class PaginationModel
    {
        public int PageSize { get; internal set; }

        public int CurrentPage { get; internal set; }

        public int PageCount { get; internal set; }

        public int TotalItemCount { get; internal set; }

        public IList<PaginationLink> PaginationLinks { get; private set; }
        
        public PagerOptions Options { get; internal set; }

        public PaginationModel()
        {
            PaginationLinks = new List<PaginationLink>();
            Options = null;
        }
    }
}
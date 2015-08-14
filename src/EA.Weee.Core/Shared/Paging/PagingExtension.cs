namespace EA.Weee.Core.Shared.Paging
{
    using System.Collections.Generic;
    using System.Linq;

    public static class PagingExtension
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize,
          int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize,
            int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }
    }
}

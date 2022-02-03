namespace EA.Weee.Api.Client.Entities
{
    using System.Collections.Generic;

    public class PagedErrorDataList
    {
        public IList<ErrorData> Errors { get; set; }

        public int TotalRecords { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
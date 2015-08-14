namespace EA.Weee.Web.Infrastructure.Paging
{
    public class PaginationLink
    {
        public bool Active { get; set; }

        public bool IsCurrent { get; set; }

        public int? PageIndex { get; set; }

        public string DisplayText { get; set; }

        public string DisplayTitle { get; set; }

        public string Url { get; set; }

        public bool IsSpacer { get; set; }
    }
}
namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.Search;
    using System;

    [Serializable]
    public class ReturnAatfAddressResult : SearchResult
    {        
        public Guid SearchTermId { get; set; }

        public string SearchTermName { get; set; }
    }
}

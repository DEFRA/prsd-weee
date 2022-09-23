namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetSearchAatfAddress : IRequest<List<ReturnAatfAddressResult>>
    {
        public string SearchTerm { get; set; }

        public Guid CurrentSelectedAatfId { get; set; }

        public bool IsGeneralSearch { get; set; }

        public GetSearchAatfAddress(string searchTerm, Guid currentSelectedAatfId, bool isGeneralSearch = false)
        {
            this.SearchTerm = searchTerm;
            this.CurrentSelectedAatfId = currentSelectedAatfId;
            this.IsGeneralSearch = isGeneralSearch;
        }
    }
}

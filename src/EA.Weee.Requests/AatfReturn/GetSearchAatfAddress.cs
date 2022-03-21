namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetSearchAatfAddress : IRequest<List<ReturnAatfAddressData>>
    {
        public string SearchTerm { get; set; }

        public GetSearchAatfAddress(string searchTerm)
        {
            this.SearchTerm = searchTerm;
        }
    }
}

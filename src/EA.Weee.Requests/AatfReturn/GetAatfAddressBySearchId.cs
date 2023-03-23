namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAatfAddressBySearchId : IRequest<List<WeeeSearchedAnAatfListData>>
    {
        public Guid SearchedAatfId { get; set; }

        public string SearchedTerm { get; set; }

        public Guid CurrentSelectedAatfId { get; set; }

        public GetAatfAddressBySearchId(Guid searchedAatfId, string searchedTerm, Guid currentSelectedAatfId)
        {
            SearchedAatfId = searchedAatfId;
            SearchedTerm = searchedTerm;
            CurrentSelectedAatfId = currentSelectedAatfId;
        }
    }
}

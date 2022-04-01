namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetAatfAddressBySearchId : IRequest<List<WeeeSearchedAnAatfListData>>
    {
        public Guid SearchedAatfId { get; set; }

        public GetAatfAddressBySearchId(Guid searchedAatfId)
        {
            SearchedAatfId = searchedAatfId;
        }
    }
}

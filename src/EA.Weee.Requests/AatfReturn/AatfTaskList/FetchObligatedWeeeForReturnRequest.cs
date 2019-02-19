namespace EA.Weee.Requests.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class FetchObligatedWeeeForReturnRequest : IRequest<List<decimal?>>
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationsId { get; set; }
        
        public FetchObligatedWeeeForReturnRequest(Guid returnId, Guid organisationId)
        {
            ReturnId = returnId;
            OrganisationsId = organisationId;
        }
    }
}

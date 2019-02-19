namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class FetchNonObligatedWeeeForReturnRequest : IRequest<List<decimal?>>
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationsId { get; set; }

        public bool Dcf { get; set; }

        public FetchNonObligatedWeeeForReturnRequest(Guid returnId, Guid organisationId, bool dcf)
        {
            ReturnId = returnId;
            OrganisationsId = organisationId;
            Dcf = dcf;
        }
    }
}

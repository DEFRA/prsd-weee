namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetReturns : IRequest<IList<ReturnData>>
    {
        public Guid OrganisationId { get; private set; }

        public FacilityType Facility { get; private set; }

        public GetReturns(Guid organisationId, FacilityType facility)
        {
            this.OrganisationId = organisationId;
            this.Facility = facility;
        }
    }
}

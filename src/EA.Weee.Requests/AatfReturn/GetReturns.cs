namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;

    public class GetReturns : IRequest<ReturnsData>
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

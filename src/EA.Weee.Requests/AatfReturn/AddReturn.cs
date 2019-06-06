namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class AddReturn : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}

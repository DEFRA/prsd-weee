namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core.Mediator;
    using System;

    public class AddReturn : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public int Year { get; set; }

        public QuarterType Quarter { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}

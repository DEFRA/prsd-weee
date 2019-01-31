namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Prsd.Core.Mediator;

    public class AddReturnRequest : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }
    }
}

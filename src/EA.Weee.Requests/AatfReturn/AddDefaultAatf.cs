namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Prsd.Core.Mediator;

    public class AddDefaultAatf : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
    }
}

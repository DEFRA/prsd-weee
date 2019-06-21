namespace EA.Weee.Requests.Admin
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class CompleteOrganisationAdmin : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
    }
}

namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;

    public class CompleteRegistration : IRequest<Guid>
    {
        public CompleteRegistration(Guid id)
        {
            OrganisationId = id;
        }

        public Guid OrganisationId { get; set; }
    }
}

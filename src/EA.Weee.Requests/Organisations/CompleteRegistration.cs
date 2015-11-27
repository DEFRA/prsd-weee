namespace EA.Weee.Requests.Organisations
{
    using Prsd.Core.Mediator;
    using System;

    public class CompleteRegistration : IRequest<Guid>
    {
        public CompleteRegistration(Guid id)
        {
            OrganisationId = id;
        }

        public Guid OrganisationId { get; set; }
    }
}

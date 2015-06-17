namespace EA.Weee.Web.Controllers
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class JoinOrganisation : IRequest<Guid>
    {
        public string UserId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public JoinOrganisation(string userId, Guid organisationId)
        {
            UserId = userId;
            OrganisationId = organisationId;
        }
    }
}
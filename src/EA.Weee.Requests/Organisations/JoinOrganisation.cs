namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class JoinOrganisation : IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public Guid OrganisationId { get; set; }

        public JoinOrganisation(Guid userId, Guid organisationToJoin)
        {
            UserId = userId;
            OrganisationId = organisationToJoin;
        }
    }
}
namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class JoinOrganisation : IRequest<Guid>
    {
        public string UserId { get; set; }

        public Guid OrganisationId { get; set; }

        public JoinOrganisation(string userId, Guid organisationToJoin)
        {
            UserId = userId;
            OrganisationId = organisationToJoin;
        }
    }
}
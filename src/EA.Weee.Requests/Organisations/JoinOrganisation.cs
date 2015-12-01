namespace EA.Weee.Requests.Organisations
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class JoinOrganisation : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }
        
        public JoinOrganisation(Guid organisationToJoin)
        {
            OrganisationId = organisationToJoin;
        }
    }
}
namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class JoinOrganisation : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }
        
        public JoinOrganisation(Guid organisationToJoin)
        {
            OrganisationId = organisationToJoin;
        }
    }
}
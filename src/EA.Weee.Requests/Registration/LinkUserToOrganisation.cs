namespace EA.Weee.Requests.Registration
{
    using System;
    using Prsd.Core.Mediator;

    public class LinkUserToOrganisation : IRequest<bool>
    {
        private readonly Guid organisationId;

        public LinkUserToOrganisation(Guid organisationId)
        {
            this.organisationId = organisationId;
        }

        public Guid OrganisationId
        {
            get { return organisationId; }
        }
    }
}
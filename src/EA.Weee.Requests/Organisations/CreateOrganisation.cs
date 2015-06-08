namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;

    public class CreateOrganisation : IRequest<Guid>
    {
        public CreateOrganisation(OrganisationData organisation)
        {
            Organisation = organisation;
        }

        public OrganisationData Organisation { get; set; }
    }
}

namespace EA.Weee.Requests.Registration
{
    using System;
    using Prsd.Core.Mediator;

    public class CreateOrganisation : IRequest<Guid>
    {
        public CreateOrganisation(OrganisationRegistrationData organisation)
        {
            Organisation = organisation;
        }

        public OrganisationRegistrationData Organisation { get; set; }
    }
}
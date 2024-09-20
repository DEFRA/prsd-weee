namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using System;

    public class OrganisationByRegistrationNumberValue : IRequest<OrganisationData>
    {
        public string RegistrationNumber { get; private set; }

        public OrganisationByRegistrationNumberValue(string registrationNumber)
        {
            Guard.ArgumentNotDefaultValue(() => registrationNumber, registrationNumber);

            RegistrationNumber = registrationNumber;
        }
    }
}

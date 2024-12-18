namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class OrganisationByRegistrationNumberValue : IRequest<List<OrganisationData>>
    {
        public string RegistrationNumber { get; private set; }

        public OrganisationByRegistrationNumberValue(string registrationNumber)
        {
            Guard.ArgumentNotDefaultValue(() => registrationNumber, registrationNumber);

            RegistrationNumber = registrationNumber;
        }
    }
}

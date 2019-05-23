namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class CreateOrganisationAdmin : IRequest<Guid>
    {
        public OrganisationType OrganisationType { get; set; }

        public AddressData Address { get; set; }

        public string BusinessName { get; set; }

        public string TradingName { get; set; }

        public string RegistrationNumber { get; set; }
    }
}

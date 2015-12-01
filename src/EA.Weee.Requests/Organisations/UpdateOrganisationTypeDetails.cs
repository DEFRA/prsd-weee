namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class UpdateOrganisationTypeDetails : IRequest<Guid>
    {
        public UpdateOrganisationTypeDetails(Guid id, OrganisationType type, string name, string tradingName, string companiesRegistrationNumber)
        {
            OrganisationId = id;
            OrganisationType = type;
            Name = name;
            TradingName = tradingName;
            CompaniesRegistrationNumber = companiesRegistrationNumber;
        }

        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string TradingName { get; set; }

        public string CompaniesRegistrationNumber { get; set; }

        public OrganisationType OrganisationType { get; set; }
    }
}

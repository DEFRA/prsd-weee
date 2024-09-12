namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;
    using CuttingEdge.Conditions;

    public class RepresentedOrganisationDetailsRequest : IRequest<bool>
    {
        public RepresentedOrganisationDetailsRequest(
            Guid directRegistrantId, 
            string companyName, 
            string businessTradingName, 
            RepresentingCompanyAddressData address)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(companyName).IsNotNullOrWhiteSpace();
            Condition.Requires(address).IsNotNull();

            CompanyName = companyName;
            BusinessTradingName = businessTradingName;
            DirectRegistrantId = directRegistrantId;

            Address = address;
        }

        public Guid DirectRegistrantId { get; set; }

        public string CompanyName { get; set; }

        public string BusinessTradingName { get; set; }

        public RepresentingCompanyAddressData Address { get; set; }
    }
}

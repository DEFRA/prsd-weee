namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;

    public class EditContactDetailsRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public string CompanyName { get; private set; }

        public string TradingName { get; private set; }

        public string EEEBrandNames {get; private set; }

        public AddressData BusinessAddressData {get; private set; }

        public EditContactDetailsRequest(Guid directRegistrantId, string companyName, string tradingName, AddressData businessAddressData, string eeeBrandNames)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(companyName).IsNotNullOrWhiteSpace();
            Condition.Requires(businessAddressData).IsNotNull();

            DirectRegistrantId = directRegistrantId;
            CompanyName = companyName;
            TradingName = tradingName;
            BusinessAddressData = businessAddressData;
            EEEBrandNames = eeeBrandNames;
        }
    }
}

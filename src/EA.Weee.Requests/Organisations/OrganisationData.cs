namespace EA.Weee.Requests.Organisations
{
    using System;
    using Shared;

    public class OrganisationData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public AddressData OrganisationAddress { get; set; }

        public AddressData BusinessAddress { get; set; }

        public AddressData NotificationAddress { get; set; }

        public ContactData Contact { get; set; }
    }
}

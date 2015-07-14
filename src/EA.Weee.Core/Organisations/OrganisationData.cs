namespace EA.Weee.Core.Organisations
{
    using System;
    using Shared;

    public class OrganisationData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public AddressData OrganisationAddress { get; set; }

        public AddressData BusinessAddress { get; set; }

        public AddressData NotificationAddress { get; set; }

        public ContactData Contact { get; set; }

        public bool HasOrganisationAddress { get; set; }

        public bool HasBusinessAddress { get; set; }

        public bool HasNotificationAddress { get; set; }
    }
}

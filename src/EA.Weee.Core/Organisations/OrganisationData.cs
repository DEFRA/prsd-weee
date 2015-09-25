namespace EA.Weee.Core.Organisations
{
    using System;
    using Shared;

    public class OrganisationData
    {
        public Guid Id { get; set; }
        public byte[] RowVersion { get; set; }

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

        /// <summary>
        /// Provides a name that can be displayed to identify the organisation.
        /// Where the organisation is a registered company this is simply the name,
        /// otherwise this is the trading name.
        /// </summary>
        public string OrganisationName { get; set; }
    }
}

namespace EA.Weee.Core.Organisations
{
    using Shared;
    using System;

    [Serializable]
    public class OrganisationData
    {
        public virtual Guid Id { get; set; }

        public Guid? SchemeId { get; set; }

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

        public bool HasBusinessAddress { get; set; }

        public bool HasNotificationAddress { get; set; }

        public bool HasAatfs { get; set; }

        public bool HasAes { get; set; }

        /// <summary>
        /// Provides a name that can be displayed to identify the organisation.
        /// Where the organisation is a registered company this is simply the name,
        /// otherwise this is the trading name.
        /// </summary>
        public string OrganisationName { get; set; }

        public bool CanEditOrganisation { get; set; }

        public bool IsRegisteredCompany => OrganisationType == OrganisationType.RegisteredCompany;

        public bool IsBalancingScheme { get; set; }
    }
}

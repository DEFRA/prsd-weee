namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        public Organisation(OrganisationType organisationType)
        {
            Guard.ArgumentNotNull(organisationType);

            OrganisationType = organisationType;
            OrganisationStatus = OrganisationStatus.Incomplete;
        }

        protected Organisation()
        {
        }

        public string Name { get; set; }

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public Address OrganisationAddress { get; set; }

        public Address BusinessAddress { get; set; }

        public Address NotificationAddress { get; set; }

        public Contact Contact { get; set; }
    }
}
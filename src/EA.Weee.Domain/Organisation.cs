namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        public Organisation(OrganisationType organisationType, OrganisationStatus organisationStatus)
        {
            Guard.ArgumentNotNull(organisationType);
            Guard.ArgumentNotNull(organisationStatus);

            OrganisationType = organisationType;
            OrganisationStatus = organisationStatus;
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
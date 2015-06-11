namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        public Organisation(string name, OrganisationType organisationType, OrganisationStatus organisationStatus)
        {
            Guard.ArgumentNotNull(name);
            Guard.ArgumentNotNull(organisationType);
            Guard.ArgumentNotNull(organisationStatus);

            Name = name;
            OrganisationType = organisationType;
            OrganisationStatus = organisationStatus;
        }

        protected Organisation()
        {
        }

        public string Name { get; private set; }

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public virtual Address OrganisationAddress { get; set; }

        public virtual Address BusinessAddress { get; set; }

        public virtual Address NotificationAddress { get; set; }

        public virtual Contact Contact { get; set; }
    }
}
namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        public Organisation(string name, string tradingName, OrganisationType organisationType, OrganisationStatus organisationStatus)
        {
            Guard.ArgumentNotNull(organisationType);
            Guard.ArgumentNotNull(organisationStatus);

            if (organisationType == OrganisationType.RegisteredCompany)
            {
                Guard.ArgumentNotNull(name);
                Name = name;
                TradingName = tradingName;
            }
            else
            {
                Guard.ArgumentNotNull(tradingName);
                TradingName = tradingName;
            }

            OrganisationType = organisationType;
            OrganisationStatus = organisationStatus;
        }

        protected Organisation()
        {
        }

        public string DisplayName
        {
            get
            {
                return OrganisationType == OrganisationType.RegisteredCompany ? Name : TradingName;
            }
        }

        public string Name { get; private set; }

        public string TradingName { get; private set; }

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string CompanyRegistrationNumber { get; set; }

<<<<<<< HEAD
        public virtual Address OrganisationAddress { get; set; }

        public virtual Address BusinessAddress { get; set; }

        public virtual Address NotificationAddress { get; set; }
=======
        public virtual Address OrganisationAddress { get; private set; }

        public virtual Address BusinessAddress { get; private set; }

        public virtual Address NotificationAddress { get; private set; }
>>>>>>> develop

        public virtual Contact Contact { get; private set; }
    }
}
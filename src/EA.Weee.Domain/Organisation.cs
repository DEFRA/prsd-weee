﻿namespace EA.Weee.Domain
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

        public Address OrganisationAddress { get; set; }

        public Address BusinessAddress { get; set; }

        public Address NotificationAddress { get; set; }

        public Contact Contact { get; set; }
    }
}
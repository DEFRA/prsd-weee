namespace EA.Weee.Domain.Organisation
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        private string companyRegistrationNumber;

        private Organisation(OrganisationType organisationType, string tradingName)
        {
            Guard.ArgumentNotNull(() => organisationType, organisationType);
            Guard.ArgumentNotNullOrEmpty(() => tradingName, tradingName);

            OrganisationType = organisationType;
            TradingName = tradingName;
            OrganisationStatus = OrganisationStatus.Incomplete;
        }

        private Organisation(OrganisationType organisationType, string companyName, string companyRegistrationNumber, string tradingName = null)
        {
            Guard.ArgumentNotNull(() => organisationType, organisationType);
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
            Guard.ArgumentNotNullOrEmpty(() => companyRegistrationNumber, companyRegistrationNumber);

            OrganisationType = organisationType;
            OrganisationStatus = OrganisationStatus.Incomplete;
            Name = companyName;
            CompanyRegistrationNumber = companyRegistrationNumber;
            TradingName = tradingName;
        }

        protected Organisation()
        {
        }

        public string Name { get; private set; }

        public virtual OrganisationType OrganisationType { get; private set; }

        public virtual OrganisationStatus OrganisationStatus { get; set; }

        public virtual string TradingName { get; private set; }

        public string CompanyRegistrationNumber
        {
            get { return companyRegistrationNumber; }
            private set
            {
                if (value != null && (value.Length < 7 || value.Length > 15))
                {
                    throw new InvalidOperationException("Company registration number must be 7 to 15 characters long");
                }

                companyRegistrationNumber = value;
            }
        }

        public virtual Address OrganisationAddress { get; private set; }

        public virtual Guid? OrganisationAddressId { get; private set; }

        public virtual Address BusinessAddress { get; private set; }

        public virtual Guid? BusinessAddressId { get; private set; }

        public virtual Address NotificationAddress { get; private set; }

        public virtual Guid? NotificationAddressId { get; private set; }

        public virtual Contact Contact { get; private set; }

        public virtual Guid? ContactId { get; private set; }

        public static Organisation CreateSoleTrader(string tradingName)
        {
            return new Organisation(OrganisationType.SoleTraderOrIndividual, tradingName);
        }

        public static Organisation CreatePartnership(string tradingName)
        {
            return new Organisation(OrganisationType.Partnership, tradingName);
        }

        public static Organisation CreateRegisteredCompany(string companyName, string companyRegistrationNumber, string tradingName = null)
        {
            return new Organisation(OrganisationType.RegisteredCompany, companyName, companyRegistrationNumber, tradingName);
        }

        public void UpdateOrganisationTypeDetails(string companyName, string companyRegNumber,
            string tradingName, OrganisationType organisationType)
        {
            Guard.ArgumentNotNull(() => organisationType, organisationType);
            if (organisationType == OrganisationType.SoleTraderOrIndividual)
            {
                Guard.ArgumentNotNullOrEmpty(() => tradingName, tradingName);
            }
            else if (organisationType == OrganisationType.Partnership)
            {
                Guard.ArgumentNotNullOrEmpty(() => tradingName, tradingName);
            }
            else if (organisationType == OrganisationType.RegisteredCompany)
            {
                Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
                Guard.ArgumentNotNullOrEmpty(() => companyRegNumber, companyRegNumber);
                Name = companyName;
                CompanyRegistrationNumber = companyRegNumber;
            }

            OrganisationType = organisationType;
            TradingName = tradingName;
        }

        public void UpdateSoleTraderOrIndividualDetails(string tradingName)
        {
            Guard.ArgumentNotNullOrEmpty(() => tradingName, tradingName);
            TradingName = tradingName;
        }

        public void UpdateRegisteredCompanyDetails(string companyName, string companyRegNumber, string tradingName)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
            Guard.ArgumentNotNullOrEmpty(() => companyRegNumber, companyRegNumber);
            Name = companyName;
            CompanyRegistrationNumber = companyRegNumber;
            TradingName = tradingName;
        }

        public void CompleteRegistration()
        {
            ToComplete();
        }

        private void ToComplete()
        {
            if (OrganisationStatus != OrganisationStatus.Incomplete)
            {
                throw new InvalidOperationException("Organisation status must be Incomplete to transition to Complete");
            }

            if (OrganisationAddress == null)
            {
                throw new InvalidOperationException("A Complete organisation must have an OrganisationAddress");
            }

            if (Contact == null)
            {
                throw new InvalidOperationException("A Complete organisation must have a Contact");
            }

            OrganisationStatus = OrganisationStatus.Complete;
        }

        public bool HasOrganisationAddress
        {
            get { return OrganisationAddress != null; }
        }

        public bool HasBusinessAddress
        {
            get { return BusinessAddress != null; }
        }

        public bool HasNotificationAddress
        {
            get { return NotificationAddress != null; }
        }

        /// <summary>
        /// Provides a name that can be displayed to identify the organisation.
        /// Where the organisation is a registered company this is simply the name,
        /// otherwise this is the trading name.
        /// </summary>
        public string OrganisationName
        {
            get
            {
                if (OrganisationType.Value == OrganisationType.RegisteredCompany.Value)
                {
                    return Name;
                }
                else
                {
                    return TradingName;
                }
            }
        }
    }
}
namespace EA.Weee.Domain
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Organisation : Entity
    {
        private string companyRegistrationNumber;

        private Organisation(OrganisationType organisationType, string tradingName)
        {
            Guard.ArgumentNotNull(organisationType);
            Guard.ArgumentNotNull(tradingName);

            OrganisationType = organisationType;
            TradingName = tradingName;
            OrganisationStatus = OrganisationStatus.Incomplete;
        }

        private Organisation(OrganisationType organisationType, string companyName, string companyRegistrationNumber, string tradingName = null)
        {
            Guard.ArgumentNotNull(organisationType);
            Guard.ArgumentNotNull(companyName);
            Guard.ArgumentNotNull(companyRegistrationNumber);

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

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; private set; }

        public string CompanyRegistrationNumber
        {
            get { return companyRegistrationNumber; }
            private set
            {
                if (value.Length < 7 || value.Length > 8)
                {
                    throw new InvalidOperationException("Company registration number must be 7 or 8 characters");
                }

                companyRegistrationNumber = value;
            }
        }

        public virtual Address OrganisationAddress { get; private set; }

        public virtual Address BusinessAddress { get; private set; }

        public virtual Address NotificationAddress { get; private set; }

        public virtual Contact Contact { get; private set; }

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
    }
}
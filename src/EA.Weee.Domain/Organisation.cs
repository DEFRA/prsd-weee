namespace EA.Weee.Domain
{
    using System;
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

        public string Name { get; private set; }

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; private set; }

        public string CompanyRegistrationNumber { get; private set; }

        public Address OrganisationAddress { get; set; }

        public Address BusinessAddress { get; set; }

        public Address NotificationAddress { get; set; }

        public Contact Contact { get; set; }

        public static Organisation CreateSoleTrader(string tradingName)
        {
            return new Organisation(OrganisationType.SoleTraderOrIndividual)
            {
                TradingName = tradingName
            };
        }

        public static Organisation CreatePartnership(string tradingName)
        {
            return new Organisation(OrganisationType.Partnership)
            {
                TradingName = tradingName
            };
        }

        public static Organisation CreateRegisteredCompany(string companyName, string companyRegistrationNumber, string tradingName = null)
        {
            if (companyRegistrationNumber.Length < 7 || companyRegistrationNumber.Length > 8)
            {
                throw new Exception("Company registration number must be 7 or 8 characters");
            }

            return new Organisation(OrganisationType.RegisteredCompany)
            {
                Name = companyName,
                CompanyRegistrationNumber = companyRegistrationNumber,
                TradingName = tradingName
            };
        }
    }
}
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

        public OrganisationType OrganisationType { get; private set; }

        public OrganisationStatus OrganisationStatus { get; set; }

        public string TradingName { get; private set; }

        public string CompanyRegistrationNumber
        {
            get { return companyRegistrationNumber; }
            private set
            {
                if (value != null && (value.Length < 7 || value.Length > 8))
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
                Guard.ArgumentNotNullOrEmpty(() => companyRegistrationNumber, companyRegistrationNumber);
            }

            OrganisationType = organisationType;
            Name = companyName;
            CompanyRegistrationNumber = companyRegNumber;
            TradingName = tradingName;
        }

        public void CompleteRegistration()
        {
            ToPending();
        }

        private void ToPending()
        {
            if (OrganisationStatus != OrganisationStatus.Incomplete)
            {
                throw new InvalidOperationException("Organisation status must be Incomplete to transition to Pending");
            }

            if (OrganisationAddress == null)
            {
                throw new InvalidOperationException("A Pending organisation must have an OrganisationAddress");
            }

            if (Contact == null)
            {
                throw new InvalidOperationException("A Pending organisation must have a Contact");
            }

            OrganisationStatus = OrganisationStatus.Pending;
        }

        public void ToApproved()
        {
            if (OrganisationStatus != OrganisationStatus.Pending)
            {
                throw new InvalidOperationException("Organisation status must be Pending to transition to Approved");
            }

            // insert other guards as needed...

            OrganisationStatus = OrganisationStatus.Approved;
        }

        public void ToRefused()
        {
            if (OrganisationStatus != OrganisationStatus.Pending)
            {
                throw new InvalidOperationException("Organisation status must be Pending to transition to Refused");
            }

            OrganisationStatus = OrganisationStatus.Refused;
        }

        public void ToWithdrawn()
        {
            if (OrganisationStatus != OrganisationStatus.Approved)
            {
                throw new InvalidOperationException("Organisation status must be Approved to transition to Withdrawn");
            }

            OrganisationStatus = OrganisationStatus.Withdrawn;
        }
    }
}
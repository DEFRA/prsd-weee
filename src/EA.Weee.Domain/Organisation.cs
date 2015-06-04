namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class Organisation : Entity
    {
        public Organisation(string name, string type)
        {
            Guard.ArgumentNotNull(name);
            Guard.ArgumentNotNull(type);

            Name = name;
            Type = type;
        }

        protected Organisation()
        {
        }

        public string Name { get; private set; }

        public string Type { get; private set; }

        public string Status { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public Address OrganisationAddress { get; set; }

        public Address BusinessAddress { get; set; }

        public Address NotificationAddress { get; set; }

        public Contact Contact { get; set; }
    }
}
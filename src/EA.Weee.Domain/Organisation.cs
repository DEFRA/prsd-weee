namespace EA.Weee.Domain
{
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Organisation : Entity
    {
        public Organisation(string name, Address address, string type, string registrationNumber = null)
        {
            Guard.ArgumentNotNull(name);
            Guard.ArgumentNotNull(address);
            Guard.ArgumentNotNull(type);

            Name = name;
            Address = address;
            Type = type;
            RegistrationNumber = registrationNumber;
        }

        private Organisation()
        {
        }

        public string Name { get; private set; }

        public Address Address { get; private set; }

        public string Type { get; private set; }

        public string RegistrationNumber { get; private set; }
    }
}
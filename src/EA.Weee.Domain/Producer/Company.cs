namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class Company : Entity
    {
        public Company(string name, string registrationNumber, Contact registeredOfficeAddress)
        {
            Name = name;
            RegistrationNumber = registrationNumber;
            RegisteredOfficeAddress = registeredOfficeAddress;
        }

        public string Name { get; private set; }

        public string RegistrationNumber { get; private set; }

        public Contact RegisteredOfficeAddress { get; private set; }
    }
}

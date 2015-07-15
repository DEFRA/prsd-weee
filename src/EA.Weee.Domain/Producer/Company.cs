namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class Company : Entity
    {
        public Company(string name, string registrationNumber, ProducerContact registeredOfficeAddress)
        {
            Name = name;
            RegistrationNumber = registrationNumber;
            RegisteredOfficeAddress = registeredOfficeAddress;
        }

         protected Company()
        {
        }

        public string Name { get; private set; }

        public string RegistrationNumber { get; private set; }

        public ProducerContact RegisteredOfficeAddress { get; private set; }
    }
}

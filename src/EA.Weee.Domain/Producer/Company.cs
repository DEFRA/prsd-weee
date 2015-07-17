namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class Company : Entity
    {
        public Company(string name, string registrationNumber, ProducerContact registeredOfficeContact)
        {
            Name = name;
            CompanyNumber = registrationNumber;
            RegisteredOfficeContact = registeredOfficeContact;
        }

         protected Company()
        {
        }

        public string Name { get; private set; }

        public string CompanyNumber { get; private set; }

        public virtual Guid RegisteredOfficeContactId { get; private set; }
        public virtual ProducerContact RegisteredOfficeContact { get; private set; }
    }
}

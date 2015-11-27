namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class Company : Entity, IEquatable<Company>
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

         public override int GetHashCode()
         {
             return base.GetHashCode();
         }

         public virtual bool Equals(Company other)
         {
             if (other == null)
             {
                 return false;
             }

             return Name == other.Name &&
                    CompanyNumber == other.CompanyNumber &&
                    object.Equals(RegisteredOfficeContact, other.RegisteredOfficeContact);
         }

         public override bool Equals(Object obj)
         {
             return Equals(obj as Company);
         }

        public string Name { get; private set; }

        public string CompanyNumber { get; private set; }

        public virtual Guid RegisteredOfficeContactId { get; private set; }
        public virtual ProducerContact RegisteredOfficeContact { get; private set; }
    }
}

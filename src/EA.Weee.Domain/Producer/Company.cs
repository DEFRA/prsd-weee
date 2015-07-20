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

         public override int GetHashCode()
         {
             return base.GetHashCode();
         }

         public override bool Equals(Object obj)
         {
             Company companyObj = obj as Company;
             if (companyObj == null)
             {
                 return false;
             }
             else
             {
                 return Name.Equals(companyObj.Name)
                        && CompanyNumber.Equals(companyObj.CompanyNumber)
                        && RegisteredOfficeContact != null && RegisteredOfficeContact.Equals(companyObj.RegisteredOfficeContact);
             }
         }

        public string Name { get; private set; }

        public string CompanyNumber { get; private set; }

        public virtual Guid RegisteredOfficeContactId { get; private set; }
        public virtual ProducerContact RegisteredOfficeContact { get; private set; }
    }
}

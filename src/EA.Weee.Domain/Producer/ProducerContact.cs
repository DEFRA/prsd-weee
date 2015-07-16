namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerContact : Entity
    {
        public ProducerContact(string title, string forename, string surname, string telephone, string mobile, string fax, string email, ProducerAddress address)
        {
            Title = title;
            ForeName = forename;
            SurName = surname;
            Telephone = telephone;
            Fax = fax;
            Email = email;
            Address = address;
            Mobile = mobile;
        }

         protected ProducerContact()
        {
        }

        public string Title { get; private set; }

        public string ForeName { get; private set; }

        public string SurName { get; private set; }

        public string Telephone { get; private set; }

        public string Mobile { get; private set; }

        public string Fax { get; private set; }

        public string Email { get; private set; }

        public virtual Guid AddressId { get; private set; }
        public virtual ProducerAddress Address { get; private set; }
    }
}

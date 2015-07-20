namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerContact : Entity
    {
        public ProducerContact(string title, string forename, string surname, string telephone, string mobile,
            string fax, string email, ProducerAddress address)
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            var contactObj = obj as ProducerContact;
            if (contactObj == null)
            {
                return false;
            }
            return Title.Equals(contactObj.Title)
                   && ForeName.Equals(contactObj.ForeName)
                   && SurName.Equals(contactObj.SurName)
                   && Telephone.Equals(contactObj.Telephone)
                   && Mobile.Equals(contactObj.Mobile)
                   && Fax.Equals(contactObj.Fax)
                   && Email.Equals(contactObj.Email)
                   && (Address != null && Address.Equals(contactObj.Address));
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

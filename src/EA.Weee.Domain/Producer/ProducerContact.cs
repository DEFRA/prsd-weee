namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerContact : Entity, IEquatable<ProducerContact>
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
            return Equals(obj as ProducerContact);
        }

        public virtual bool Equals(ProducerContact other)
        {
            if (other == null)
            {
                return false;
            }

            return Title == other.Title
                   && ForeName == other.ForeName
                   && SurName == other.SurName
                   && Telephone == other.Telephone
                   && Mobile == other.Mobile
                   && Fax == other.Fax
                   && Email == other.Email
                   && object.Equals(Address, other.Address);
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

        public virtual bool IsOverseas
        {
            get { return !Address.IsUkAddress(); }
        }

        public string ContactName
        {
            get { return string.Format("{0} {1} {2}", Title, ForeName, SurName); }
        }
    }
}

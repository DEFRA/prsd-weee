namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class ProducerContact : Entity
    {
        public ProducerContact(string title, string forename, string surname, string telephone, string mobile, string fax, string email, ProducerAddress address)
        {
            Title = title;
            ForeName = forename;
            SurName = surname;
            Landline = telephone;
            Fax = fax;
            Email = email;
            Address = address;
            Mobile = mobile;
        }

        public string Title { get; private set; }

        public string ForeName { get; private set; }

        public string SurName { get; private set; }

        public string Landline { get; private set; }

        public string Mobile { get; private set; }

        public string Fax { get; private set; }

        public string Email { get; private set; }

        public virtual ProducerAddress Address { get; private set; }
    }
}

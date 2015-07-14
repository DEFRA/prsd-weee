namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class Contact : Entity
    {
        public Contact(string title, string forename, string surname, string telephone, string mobile, string fax, string email, Address address)
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

        public virtual Address Address { get; private set; }
    }
}

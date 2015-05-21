namespace EA.Weee.Requests.Registration.Users
{
    using Organisations;

    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public virtual OrganisationData Organisation { get; set; }
    }
}
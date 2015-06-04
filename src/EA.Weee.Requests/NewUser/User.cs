namespace EA.Weee.Requests.NewUser
{
    using Organisations;

    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public virtual OrganisationSearchData Organisation { get; set; }
    }
}
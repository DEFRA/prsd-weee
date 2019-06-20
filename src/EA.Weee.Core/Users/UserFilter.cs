namespace EA.Weee.Core.User
{
    using EA.Weee.Core.Shared;

    public class UserFilter
    {
        public string Name { get; set; }

        public string OrganisationName { get; set; }

        public UserStatus? Status { get; set; }
    }
}

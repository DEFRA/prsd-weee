namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;

    public class OrganisationUserStatus : Enumeration
    {
        public static readonly OrganisationUserStatus Pending = new OrganisationUserStatus(1, "Pending");
        public static readonly OrganisationUserStatus NormalUser = new OrganisationUserStatus(2, "Normal user");
        public static readonly OrganisationUserStatus AdminUser = new OrganisationUserStatus(3, "Admin user");

        protected OrganisationUserStatus()
        {
        }

        private OrganisationUserStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
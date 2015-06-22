namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;

    public class OrganisationUserStatus : Enumeration
    {
        public static readonly OrganisationUserStatus Pending = new OrganisationUserStatus(1, "Pending");
        public static readonly OrganisationUserStatus Approved = new OrganisationUserStatus(2, "Approved");
        public static readonly OrganisationUserStatus Refused = new OrganisationUserStatus(3, "Refused");

        protected OrganisationUserStatus()
        {
        }

        private OrganisationUserStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
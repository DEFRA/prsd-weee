namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;

    public class OrganisationStatus : Enumeration
    {
        public static readonly OrganisationStatus Incomplete = new OrganisationStatus(1, "Incomplete");
        public static readonly OrganisationStatus Pending = new OrganisationStatus(2, "Pending");
        public static readonly OrganisationStatus Approved = new OrganisationStatus(3, "Approved");
        public static readonly OrganisationStatus Refused = new OrganisationStatus(4, "Refused");
        public static readonly OrganisationStatus Withdrawn = new OrganisationStatus(5, "Withdrawn");

        protected OrganisationStatus()
        {
        }

        private OrganisationStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}

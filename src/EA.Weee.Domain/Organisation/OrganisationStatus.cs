namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;

    public class OrganisationStatus : Enumeration
    {
        public static readonly OrganisationStatus Incomplete = new OrganisationStatus(1, "Incomplete");
        public static readonly OrganisationStatus Complete = new OrganisationStatus(2, "Complete");

        protected OrganisationStatus()
        {
        }

        private OrganisationStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}

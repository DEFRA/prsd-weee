namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;

    public class OrganisationAdditionalDetailsType : Enumeration
    {
        public static readonly OrganisationAdditionalDetailsType Partner = new OrganisationAdditionalDetailsType(1, "Partner");
        public static readonly OrganisationAdditionalDetailsType SoleTrader = new OrganisationAdditionalDetailsType(2, "Sole Trader");

        protected OrganisationAdditionalDetailsType()
        {
        }

        private OrganisationAdditionalDetailsType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}

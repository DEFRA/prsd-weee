namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;

    public class OrganisationType : Enumeration
    {
        public static readonly OrganisationType RegisteredCompany = new OrganisationType(1, "Registered company");
        public static readonly OrganisationType Partnership = new OrganisationType(2, "Partnership");
        public static readonly OrganisationType SoleTraderOrIndividual = new OrganisationType(3, "Sole trader or individual");

        protected OrganisationType()
        {
        }

        private OrganisationType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}

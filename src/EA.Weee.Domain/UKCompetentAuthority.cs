namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class UKCompetentAuthority : Enumeration
    {
        public static readonly UKCompetentAuthority England = new UKCompetentAuthority(1, "Environment Agency", "EA");

        public static readonly UKCompetentAuthority Scotland = new UKCompetentAuthority(2,
            "Scottish Environment Protection Agency", "SEPA");

        public static readonly UKCompetentAuthority NorthernIreland = new UKCompetentAuthority(3,
            "Northern Ireland Environment Agency", "NIEA");

        public static readonly UKCompetentAuthority Wales = new UKCompetentAuthority(4, "Natural Resources Wales", "NRW");
        private readonly string shortName;

        protected UKCompetentAuthority()
        {
        }

        private UKCompetentAuthority(int value, string displayName, string shortName)
            : base(value, displayName)
        {
            this.shortName = shortName;
        }

        public string ShortName
        {
            get { return shortName; }
        }
    }
}
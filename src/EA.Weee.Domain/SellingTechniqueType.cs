namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class SellingTechniqueType : Enumeration
    {
        public static readonly SellingTechniqueType DirectSellingtoEndUser = new SellingTechniqueType(1, "Direct Selling to End User");
        public static readonly SellingTechniqueType IndirectSellingtoEndUser = new SellingTechniqueType(2, "Indirect Selling to End User");

        protected SellingTechniqueType()
        {
        }

        private SellingTechniqueType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
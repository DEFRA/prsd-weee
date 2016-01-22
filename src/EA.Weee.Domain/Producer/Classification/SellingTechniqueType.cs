namespace EA.Weee.Domain.Producer.Classfication
{
    using Prsd.Core.Domain;

    public class SellingTechniqueType : Enumeration
    {
        public static readonly SellingTechniqueType DirectSellingtoEndUser = new SellingTechniqueType(0, "Direct Selling to End User");
        public static readonly SellingTechniqueType IndirectSellingtoEndUser = new SellingTechniqueType(1, "Indirect Selling to End User");
        public static readonly SellingTechniqueType Both = new SellingTechniqueType(2, "Both");

        protected SellingTechniqueType()
        {
        }

        private SellingTechniqueType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
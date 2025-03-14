namespace EA.Weee.Domain.Producer.Classfication
{
    using Prsd.Core.Domain;

    public class SellingTechniqueType : Enumeration
    {
        public static readonly SellingTechniqueType DirectSellingtoEndUser = new SellingTechniqueType(0, "Direct Selling to End User");
        public static readonly SellingTechniqueType IndirectSellingtoEndUser = new SellingTechniqueType(1, "Indirect Selling to End User");
        public static readonly SellingTechniqueType Both = new SellingTechniqueType(2, "Both");
        public static readonly SellingTechniqueType OnlineMarketplacesAndFulfilmentHouses = new SellingTechniqueType(3, "Online Marketplaces and Fulfilment Houses");

        protected SellingTechniqueType()
        {
        }

        private SellingTechniqueType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
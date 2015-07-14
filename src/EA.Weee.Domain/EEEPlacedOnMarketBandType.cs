namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class EEEPlacedOnMarketBandType : Enumeration
    {
        public static readonly EEEPlacedOnMarketBandType MoreThanOrEqualTo5TEEEPlacedOnMarket =
            new EEEPlacedOnMarketBandType(1, "More than or equal to 5T EEE placed on market");

        public static readonly EEEPlacedOnMarketBandType LessThan5TEEEPlacedOnMarket = new EEEPlacedOnMarketBandType(2,
            "Less than 5T EEE placed on market");

        public static readonly EEEPlacedOnMarketBandType Both = new EEEPlacedOnMarketBandType(2, "Both");

        protected EEEPlacedOnMarketBandType()
        {
        }

        private EEEPlacedOnMarketBandType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}

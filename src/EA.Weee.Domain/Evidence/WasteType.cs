namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class WasteType : Enumeration
    {
        public static readonly WasteType HouseHold = new WasteType(1, "Household");
        public static readonly WasteType NonHouseHold = new WasteType(2, "Non-household");

        protected WasteType()
        {
        }

        private WasteType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}

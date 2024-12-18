namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Domain.Lookup;

    public class TonnageData
    {
        public decimal? HouseHold { get; protected set; }

        public decimal? NonHouseHold { get; protected set; }

        public WeeeCategory CategoryId { get; protected set; }

        public TonnageData(decimal? houseHold, decimal? nonHouseHold, WeeeCategory categoryId)
        {
            HouseHold = houseHold;
            NonHouseHold = nonHouseHold;
            CategoryId = categoryId;
        }
    }
}

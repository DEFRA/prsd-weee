namespace EA.Weee.Core.Shared
{
    using DataReturns;

    public class CategoryValue
    {
        public WeeeCategory Category { get; private set; }

        public decimal? HouseHold { get; set; }

        public decimal? NonHouseHold { get; set; }

        public CategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

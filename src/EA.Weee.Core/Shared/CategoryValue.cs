namespace EA.Weee.Core.Shared
{
    using DataReturns;

    public class CategoryValue
    {
        public WeeeCategory Category { get; private set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public CategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

namespace EA.Weee.Core.Shared
{
    using DataReturns;

    public class CategoryValue
    {
        public WeeeCategory Category { get; set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public decimal NonObligated { get; set; }

        public CategoryValue()
        {
        }

        public CategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

namespace EA.Weee.Core.Shared
{
    using DataReturns;

    public class CategoryValue
    {
        public WeeeCategory Category { get; set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public string NonObligated { get; set; }

        public CategoryValue()
        {
        }

        public CategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

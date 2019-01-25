namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using Validation;

    public class CategoryValue
    {
        public WeeeCategory Category { get; set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        [TonnageValue("Category")]
        public decimal NonObligated { get; set; }

        public bool Dcf { get; set; }

        public CategoryValue()
        {
        }

        public CategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

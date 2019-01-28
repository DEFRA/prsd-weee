namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using Validation;

    public class NonObligatedCategoryValue
    {
        public WeeeCategory Category { get; set; }

        [TonnageValue("Category")]
        public decimal Tonnage { get; set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public bool Dcf { get; set; }

        public NonObligatedCategoryValue()
        {
        }

        public NonObligatedCategoryValue(WeeeCategory category)
        {
            Category = category;
        }
    }
}

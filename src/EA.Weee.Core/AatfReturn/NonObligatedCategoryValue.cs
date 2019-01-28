namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using Helpers;
    using Validation;

    public class NonObligatedCategoryValue
    {
        public string CategoryDisplay { get; set; }

        public int CategoryId { get; set; }

        [TonnageValue("CategoryId")]
        public decimal? Tonnage { get; set; }

        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public bool Dcf { get; set; }

        public NonObligatedCategoryValue()
        {
        }

        public NonObligatedCategoryValue(WeeeCategory category)
        {
            CategoryDisplay = category.ToDisplayString();
            CategoryId = (int)category;
        }
    }
}

namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using Validation;

    public class ObligatedCategoryValue : CategoryValue
    {
        [TonnageValue("CategoryId", "B2C")]
        public string B2C { get; set; }

        [TonnageValue("CategoryId", "B2B")]
        public string B2B { get; set; }

        public bool RedirectToSummary
        {
            get
            {
                return this.B2B != "-" || this.B2C != "-";
            }
        }

        public ObligatedCategoryValue()
        {
        }

        public ObligatedCategoryValue(string b2b, string b2c)
        {
            B2B = b2b;
            B2C = b2c;
        }

        public ObligatedCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}

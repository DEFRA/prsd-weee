namespace EA.Weee.Core.AatfReturn
{
    using System;
    using DataReturns;
    using EA.Weee.Core.Shared;
    using Validation;

    [Serializable]
    public class ObligatedCategoryValue : CategoryValue
    {
        [TonnageValue("CategoryId", "The tonnage value", "B2C", false)]
        public string B2C { get; set; }

        [TonnageValue("CategoryId", "The tonnage value", "B2B", false)]
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

namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Validation;

    public class ObligatedCategoryValue : CategoryValue
    {
        [TonnageValue("CategoryId", "B2C")]
        public string B2C { get; set; }

        [TonnageValue("CategoryId", "B2B")]
        public string B2B { get; set; }

        public ObligatedCategoryValue()
        {
        }

        public ObligatedCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}

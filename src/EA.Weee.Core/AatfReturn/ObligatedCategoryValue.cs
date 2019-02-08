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
        [TonnageValue("CategoryId")]
        public string HouseHold { get; set; }

        public string NonHouseHold { get; set; }

        public ObligatedCategoryValue()
        {
        }

        public ObligatedCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}

using EA.Weee.Core.DataReturns;

namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.Shared;

    public class ProducerSubmissionCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        public virtual string HouseHold { get; set; }

        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        public virtual string NonHouseHold { get; set; }

        public ProducerSubmissionCategoryValue()
        {
        }

        public ProducerSubmissionCategoryValue(string houseHold, string nonHouseHold)
        {
            HouseHold = houseHold;
            NonHouseHold = nonHouseHold;
        }

        public ProducerSubmissionCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}

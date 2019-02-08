namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;

    public class CategoryValue
    {
        public string CategoryDisplay { get; set; }

        public int CategoryId { get; set; }

        public CategoryValue()
        {
        }

        public CategoryValue(WeeeCategory category)
        {
        }
    }
}

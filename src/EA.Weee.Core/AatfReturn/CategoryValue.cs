namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;
    using Helpers;

    public class CategoryValue
    {
        public Guid Id { get; set; }

        public string CategoryDisplay { get; set; }

        public int CategoryId { get; set; }

        public CategoryValue()
        {
        }

        public CategoryValue(WeeeCategory category)
        {
            CategoryDisplay = category.ToDisplayString();
            CategoryId = (int)category;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;

    public class CategoryValues : List<CategoryValue>
    {
        public CategoryValues()
        {
            foreach (var category in GetEnumValues())
            {
                Add(new CategoryValue(category));
            }
        }

        private IEnumerable<WeeeCategory> GetEnumValues()
        {
            foreach (var item in Enum.GetValues(typeof(WeeeCategory)))
            {
                yield return (WeeeCategory)item;
            }
        }
    }
}

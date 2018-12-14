namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

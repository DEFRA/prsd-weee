﻿namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using DataReturns;

    public class CategoryValues : List<NonObligatedCategoryValue>
    {
        public CategoryValues()
        {
            foreach (var category in GetEnumValues())
            {
                Add(new NonObligatedCategoryValue(category));
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

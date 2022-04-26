namespace EA.Weee.Core.Aatf
{
    using System;
    using System.Collections.Generic;
    using DataReturns;
    using EA.Weee.Core.Shared;

    public class CategoryValues<T> : List<T> where T : CategoryValue, new()
    {
        public CategoryValues()
        {
            foreach (var category in GetEnumValues())
            {
                Add((T)Activator.CreateInstance(typeof(T), category));
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

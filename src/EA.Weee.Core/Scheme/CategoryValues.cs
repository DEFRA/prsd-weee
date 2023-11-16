namespace EA.Weee.Core.Scheme
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;

    public class CategoryValues<T> : List<T> where T : CategoryValue, new()
    {
        public CategoryValues()
        {
            foreach (var category in GetEnumValues())
            {
                Add((T)Activator.CreateInstance(typeof(T), category));
            }
        }

        public CategoryValues(List<WeeeCategory> availableCategories)
        {
            foreach (var category in GetEnumValues())
            {
                if (availableCategories.Contains(category))
                {
                    Add((T)Activator.CreateInstance(typeof(T), category));
                }
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
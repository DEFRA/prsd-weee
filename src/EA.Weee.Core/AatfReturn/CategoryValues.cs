namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using System;
    using System.Collections.Generic;

    public class CategoryValues<T> : List<T> where T : CategoryValue, new()
    {
        public CategoryValues()
        {
            foreach (var category in GetEnumValues())
            {
                Add((T)Activator.CreateInstance(typeof(T), category));
                //Add(new T(){ CategoryId = (int)category, CategoryDisplay = category.ToString()});
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

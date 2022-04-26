namespace EA.Weee.Core.Shared
{
    using DataReturns;
    using Helpers;
    using System;

    [Serializable]
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

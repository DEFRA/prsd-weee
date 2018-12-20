namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.Shared;

    public class WeeeCategoryValueViewModel
    {
        public IList<CategoryValue> CategoryValues { get; set; }

        public WeeeCategoryValueViewModel()
        {
            CategoryValues = new List<CategoryValue>();
        }

        public WeeeCategoryValueViewModel(CategoryValues categoryValues)
        {
            CategoryValues = new List<CategoryValue>();

            foreach (var categoryValue in categoryValues)
            {
                CategoryValues.Add(categoryValue);
            }
        }
    }
}
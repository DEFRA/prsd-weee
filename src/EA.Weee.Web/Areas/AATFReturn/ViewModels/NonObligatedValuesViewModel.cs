namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public class NonObligatedValuesViewModel
    {
        public IList<NonObligatedCategoryValue> CategoryValues { get; set; }

        public NonObligatedValuesViewModel()
        {
            AddCategoryValues(new CategoryValues());
        }

        public NonObligatedValuesViewModel(CategoryValues categoryValues)
        {
            AddCategoryValues(categoryValues);
        }

        private void AddCategoryValues(CategoryValues categories)
        {
            CategoryValues = new List<NonObligatedCategoryValue>();

            foreach (var categoryValue in categories)
            {
                CategoryValues.Add(categoryValue);
            }
        }
    }
}
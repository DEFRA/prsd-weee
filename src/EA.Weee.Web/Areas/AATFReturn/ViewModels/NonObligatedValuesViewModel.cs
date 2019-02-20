namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;

    public class NonObligatedValuesViewModel
    {
        public IList<NonObligatedCategoryValue> CategoryValues { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool Dcf { get; set; }

        private ICategoryValueTotalCalculator categoryValueCalculator;

        public NonObligatedValuesViewModel()
        {
            AddCategoryValues(new NonObligatedCategoryValues());
            categoryValueCalculator = new CategoryValueTotalCalculator();
        }

        public NonObligatedValuesViewModel(NonObligatedCategoryValues values)
        {
            AddCategoryValues(values);
            categoryValueCalculator = new CategoryValueTotalCalculator();
        }

        private void AddCategoryValues(NonObligatedCategoryValues nonObligatedCategories)
        {
            CategoryValues = new List<NonObligatedCategoryValue>();

            foreach (var categoryValue in nonObligatedCategories)
            {
                CategoryValues.Add(categoryValue);
            }
        }

        public string Total => categoryValueCalculator.Total(CategoryValues.Select(c => c.Tonnage).ToList());
    }
}
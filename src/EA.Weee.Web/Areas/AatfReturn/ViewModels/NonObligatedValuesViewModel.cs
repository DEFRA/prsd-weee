namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NonObligatedValuesViewModel
    {
        public IList<NonObligatedCategoryValue> CategoryValues { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool Dcf { get; set; }

        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

        public NonObligatedValuesViewModel()
        {
            this.categoryValueCalculator = new CategoryValueTotalCalculator();
        }

        public NonObligatedValuesViewModel(ICategoryValueTotalCalculator categoryValueCalculator)
        {
            AddCategoryValues(new NonObligatedCategoryValues());
            this.categoryValueCalculator = categoryValueCalculator;
        }

        public NonObligatedValuesViewModel(NonObligatedCategoryValues values, ICategoryValueTotalCalculator categoryValueCalculator)
        {
            AddCategoryValues(values);
            this.categoryValueCalculator = categoryValueCalculator;
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

        public bool Edit
        {
            get { return CategoryValues.Any(c => c.Id != Guid.Empty); }
        }

        public ICategoryValueTotalCalculator GetCalculator()
        {
            return categoryValueCalculator;
        }
    }
}
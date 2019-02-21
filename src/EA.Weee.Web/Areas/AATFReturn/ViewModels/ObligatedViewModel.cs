namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;

    public class ObligatedViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string AatfName { get; set; }

        public Guid ReturnId { get; set; }

        public IList<ObligatedCategoryValue> CategoryValues { get; set; }

        private readonly ICategoryValueTotalCalculator categoryValueCalculator;

        public ObligatedViewModel()
        {
            AddCategoryValues(new ObligatedCategoryValues());
            categoryValueCalculator = new CategoryValueTotalCalculator();
        }

        public ObligatedViewModel(ObligatedCategoryValues values)
        {
            AddCategoryValues(values);
            categoryValueCalculator = new CategoryValueTotalCalculator();
        }

        private void AddCategoryValues(ObligatedCategoryValues obligatedCategories)
        {
            CategoryValues = new List<ObligatedCategoryValue>();

            foreach (var categoryValue in obligatedCategories)
            {
                CategoryValues.Add(categoryValue);
            }
        }
        public string B2CTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.B2C).ToList());

        public string B2BTotal => categoryValueCalculator.Total(CategoryValues.Select(c => c.B2B).ToList());
    }
}
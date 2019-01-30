namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;

    public class NonObligatedValuesViewModel
    {
        public IList<NonObligatedCategoryValue> CategoryValues { get; set; }

        public Guid OrganisationId { get; set; }

        public bool Dcf { get; set; }

        public NonObligatedValuesViewModel()
        {
            AddCategoryValues(new NonObligatedCategoryValues());
        }

        public NonObligatedValuesViewModel(NonObligatedCategoryValues values)
        {
            AddCategoryValues(values);
        }

        private void AddCategoryValues(NonObligatedCategoryValues nonObligatedCategories)
        {
            CategoryValues = new List<NonObligatedCategoryValue>();

            foreach (var categoryValue in nonObligatedCategories)
            {
                CategoryValues.Add(categoryValue);
            }
        }

        public string Total
        {
            get
            {
                var total = 0.000m;
                var values = CategoryValues.Where(c => !string.IsNullOrWhiteSpace(c.Tonnage)).Select(c => c.Tonnage).ToList();

                if (values.Any())
                {
                    var convertedValues = values.ConvertAll(Convert.ToDecimal);
                    total = convertedValues.Sum();
                }

                return $"{total:0.000}";
            }
        }
    }
}
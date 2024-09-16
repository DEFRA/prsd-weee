namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EditEeeDataViewModel : IValidatableObject
    {
        public Guid OrganisationId { get; set; }
        
        public bool HasAuthorisedRepresentitive { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public IList<ProducerSubmissionCategoryValue> CategoryValues { get; set; }

        public SellingTechniqueViewModel SellingTechnique { get; set; }

        public EditEeeDataViewModel()
        {
            AddCategoryValues(new ProducerSubmissionCategoryValues());
        }

        private void AddCategoryValues(ProducerSubmissionCategoryValues evidenceCategoryValues)
        {
            CategoryValues = new List<ProducerSubmissionCategoryValue>();

            foreach (var categoryValue in evidenceCategoryValues)
            {
                CategoryValues.Add(categoryValue);
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var calculator = new CategoryValueTotalCalculator();
            if (CategoryValues != null && CategoryValues.Any())
            {
                var totalHouseHold = calculator.Total(CategoryValues.Select(t => t.HouseHold).ToList());
                var totalNonHouseHold = calculator.Total(CategoryValues.Select(t => t.NonHouseHold).ToList());
                var total = totalHouseHold.ToDecimal() + totalNonHouseHold.ToDecimal();
                if (total >= 5m)
                {
                    yield return new ValidationResult("EEE details need to total less than 5 tonnes",
                        new[] { nameof(CategoryValues) });
                }
            }
        }
    }
}
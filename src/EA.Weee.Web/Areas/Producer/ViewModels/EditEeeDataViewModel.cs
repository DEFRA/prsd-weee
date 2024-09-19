namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;

    public class EditEeeDataViewModel : IProducerSubmissionViewModel
    {
        public Guid OrganisationId { get; set; }
        
        public bool HasAuthorisedRepresentitive { get; set; }

        public Guid DirectRegistrantId { get; set; }

        [ProducerCategoryValuesValidation]
        public IList<ProducerSubmissionCategoryValue> CategoryValues { get; set; }

        [AtLeastOneChecked(nameof(SellingTechnique), nameof(SellingTechniqueViewModel.IsDirectSelling), nameof(SellingTechniqueViewModel.IsIndirectSelling), ErrorMessage = "At least one selling technique must be selected")]
        public SellingTechniqueViewModel SellingTechnique { get; set; }

        public bool? RedirectToCheckAnswers { get; set; }

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
    }
}
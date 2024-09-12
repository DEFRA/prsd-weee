namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using System;
    using System.Collections.Generic;

    public class EditEeeDataViewModel
    {
        public Guid OrganisationId { get; set; }
        
        public bool HasAuthorisedRepresentitive { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public virtual IList<ProducerSubmissionCategoryValue> CategoryValues { get; set; }

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
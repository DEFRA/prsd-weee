namespace EA.Weee.Web.Areas.AatfEvidence.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;

    public class CreateNoteViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid? ReceivedId { get; set; }

        public IList<ObligatedCategoryValue> CategoryValues { get; set; }

        public CreateNoteViewModel()
        {
            AddCategoryValues(new ObligatedCategoryValues());
        }

        private void AddCategoryValues(ObligatedCategoryValues obligatedCategories)
        {
            CategoryValues = new List<ObligatedCategoryValue>();

            foreach (var categoryValue in obligatedCategories)
            {
                CategoryValues.Add(categoryValue);
            }
        }
    }
}
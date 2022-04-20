namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class TransferEvidenceNoteDataViewModel
    {
        [Required(ErrorMessage = "Select a PCS")]
        public Guid? SelectedSchema { get; set; }

        public IList<SchemeData> SchemasToDisplay { get; set; }

        public CategoryValues<CategoryBooleanViewModel> CategoryValues { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Select a category you would like to transfer evidence from")]
        public bool HasSelectedAtLeastOneCategory => CategoryValues != null && CategoryValues.Any(c => c.Selected);

        public TransferEvidenceNoteDataViewModel()
        {
            AddCategoryValues();
        }

        public void AddCategoryValues()
        {
            CategoryValues = new CategoryValues<CategoryBooleanViewModel>();
        }
    }
}
namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class TransferEvidenceNoteCategoriesViewModel : TransferEvidenceViewModelBase
    {
        public Guid OrganisationId { get; set; }

        [Required(ErrorMessage = "Select who you would like to transfer evidence to")]
        [Display(Name = "Who would you like to transfer evidence to?")]
        public Guid? SelectedSchema { get; set; }

        public IList<EntityIdDisplayNameData> SchemasToDisplay { get; set; }

        public CategoryValues<CategoryBooleanViewModel> CategoryBooleanViewModels { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Select a category you would like to transfer evidence from")]
        public bool HasSelectedAtLeastOneCategory => CategoryBooleanViewModels != null && CategoryBooleanViewModels.Any(c => c.Selected);

        public bool SelectAllCheckboxes { get; set; } = false;

        public TransferEvidenceNoteCategoriesViewModel()
        {
            AddCategoryValues();
        }

        public void AddCategoryValues()
        {
            CategoryBooleanViewModels = new CategoryValues<CategoryBooleanViewModel>();
        }

        public List<int> SelectedCategoryValues
        {
            get
            {
                if (CategoryBooleanViewModels != null)
                {
                    return CategoryBooleanViewModels.Where(c => c.Selected).Select(c => c.CategoryId).ToList();
                }

                return new List<int>();
            }
        }
    }
}
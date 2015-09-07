namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class NotFoundOrganisationViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ActivityOptions { get; set; }

        public string SearchedText { get; set; }

        public NotFoundOrganisationViewModel()
        {
            var collection = new List<string> { NotFoundOrganisationAction.TryAnotherSearch, NotFoundOrganisationAction.CreateNewOrg };
            ActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
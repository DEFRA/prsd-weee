namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Shared;

    public class NotFoundOrganisationViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ActivityOptions { get; set; }

        public string SearchedText { get; set; }

        public string Name { get; set; }

        //public string TradingName { get; set; }

        //public string CompaniesRegistrationNumber { get; set; }

        //public OrganisationType Type { get; set; }

        //public Guid? OrganisationId { get; set; }

        public NotFoundOrganisationViewModel()
        {
            var collection = new List<string> { NotFoundOrganisationAction.TryAnotherSearch, NotFoundOrganisationAction.CreateNewOrg };
            ActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
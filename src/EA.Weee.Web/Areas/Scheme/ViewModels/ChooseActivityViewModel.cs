namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ChooseActivityViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ActivityOptions { get; set; }

        public Guid OrganisationId { get; set; }

        public ChooseActivityViewModel()
        {
            List<string> collection = new List<string> { PcsAction.ManagePcsMembers, PcsAction.ManageOrganisationUsers, PcsAction.ViewOrganisationDetails };
            ActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
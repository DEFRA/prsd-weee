namespace EA.Weee.Web.ViewModels.PCS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.ViewModels.Shared;
    
    public class ChooseActivityViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ActivityOptions { get; set; }

        public Guid OrganisationId { get; set; }

        public ChooseActivityViewModel()
        {
            List<string> collection = new List<string> { "Add / amend PCS members", "Manage organisation users" };
            ActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
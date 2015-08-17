namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRejectionViewModel
    {
        public Guid SchemeId { get; set; }

        [Required]
        public RadioButtonStringCollectionViewModel ConfirmRejectionOptions { get; set; }

        public ConfirmRejectionViewModel()
        {
            var collection = new List<string> { ConfirmSchemeRejection.Yes, ConfirmSchemeRejection.No };
            ConfirmRejectionOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRejectionViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel ConfirmRejectionOptions { get; set; }

        public ConfirmRejectionViewModel()
        {
            var collection = new List<string> { ConfirmSchemeRejectionOptions.Yes, ConfirmSchemeRejectionOptions.No };
            ConfirmRejectionOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
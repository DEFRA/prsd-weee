namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRejectionViewModel : RadioButtonStringCollectionViewModel
    {
        public ConfirmRejectionViewModel() : base(new List<string> { ConfirmSchemeRejectionOptions.Yes, ConfirmSchemeRejectionOptions.No })
        {
        }
    }
}
namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class InternalUserActivityViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel InternalUserActivityOptions { get; set; }

        public InternalUserActivityViewModel()
        {
            var collection = new List<string> { InternalUserActivity.ManageScheme, InternalUserActivity.ManageUsers };
            InternalUserActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
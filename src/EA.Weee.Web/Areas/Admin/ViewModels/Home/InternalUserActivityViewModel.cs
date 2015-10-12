namespace EA.Weee.Web.Areas.Admin.ViewModels.Home
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
            List<string> collection = new List<string>
            {
                InternalUserActivity.ManageScheme,
                InternalUserActivity.SubmissionsHistory,
                InternalUserActivity.ViewProducerInformation,
                InternalUserActivity.ManageUsers
            };
            
            InternalUserActivityOptions = new RadioButtonStringCollectionViewModel(collection);
        }
    }
}
namespace EA.Weee.Web.Areas.Admin.ViewModels.Home
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class InternalUserActivityViewModel : RadioButtonStringCollectionViewModel
    {
        public InternalUserActivityViewModel() : base(new List<string>
        {
            InternalUserActivity.ManageScheme,
            InternalUserActivity.SubmissionsHistory,
            InternalUserActivity.ProducerDetails,
            InternalUserActivity.ManageUsers,
            InternalUserActivity.ViewReports
        })
        {
        }
    }
}
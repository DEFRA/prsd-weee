﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Home
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class InternalUserActivityViewModel : RadioButtonStringCollectionViewModel
    {
        public InternalUserActivityViewModel() : base(new List<string>
        {
            InternalUserActivity.ManageScheme,
            InternalUserActivity.SubmissionsHistory,
            InternalUserActivity.ViewProducerInformation,
            InternalUserActivity.ManageUsers
        })
        {
        }
    }
}
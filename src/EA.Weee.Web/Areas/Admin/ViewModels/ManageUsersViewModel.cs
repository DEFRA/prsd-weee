namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Admin;
    using Web.ViewModels.Shared;

    public class ManageUsersViewModel
    {
        public IList<UserSearchData> Users { get; set; }
       
        public PagingViewModel UsersPagingViewModel { get; set; }

        public Guid? SelectedUserId { get; set; }
    }
}
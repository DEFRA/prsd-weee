namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Admin;
    using Web.ViewModels.Shared;

    public class ManageUsersViewModel
    {
        public List<UserSearchData> Users { get; set; }
       
        public PagingViewModel PagingViewModel { get; set; }

        public Guid? SelectedUserId { get; set; }
    }
}
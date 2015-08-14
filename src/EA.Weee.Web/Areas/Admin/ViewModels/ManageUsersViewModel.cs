namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Shared.Paging;

    public class ManageUsersViewModel
    {
        public IPagedList<UserSearchData> Users { get; set; }
        public Guid? SelectedUserId { get; set; }
    }
}
namespace EA.Weee.Web.Areas.Admin.ViewModels.User
{
    using Core.Admin;
    using Core.Shared.Paging;
    using EA.Weee.Requests.Admin;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ManageUsersViewModel
    {
        public IPagedList<UserSearchData> Users { get; set; }

        public FindMatchingUsers.OrderBy OrderBy { get; set; }

        [Required(ErrorMessage = "You must choose a user to manage")]
        public Guid? SelectedUserId { get; set; }
    }
}
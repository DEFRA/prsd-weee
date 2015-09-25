namespace EA.Weee.Web.Areas.Admin.ViewModels.User
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Admin;
    using Core.Shared.Paging;

    public class ManageUsersViewModel
    {
        public IPagedList<UserSearchData> Users { get; set; }

        [Required(ErrorMessage = "You must choose a user to manage")]
        public Guid? SelectedUserId { get; set; }
    }
}
namespace EA.Weee.Web.Areas.Admin.ViewModels.User
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Admin;
    using Core.DataStandards;
    using Core.Shared;
    using Prsd.Core.Helpers;

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            UserStatusSelectList = new SelectList(EnumHelper.GetValues(typeof(UserStatus)), "Key", "Value");
        }

        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string UserId { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public bool CanManageStatus { get; set; }

        public bool IsCompetentAuthorityUser { get; set; }

        [Required]
        [Display(Name = "Current status")]
        public UserStatus UserStatus { get; set; }

        public IEnumerable<SelectListItem> UserStatusSelectList { get; set; }

        public EditUserViewModel(ManageUserData manageUser)
        {
            Id = manageUser.Id;
            OrganisationId = manageUser.OrganisationId;
            OrganisationName = manageUser.OrganisationName;
            UserId = manageUser.UserId;
            FirstName = manageUser.FirstName;
            LastName = manageUser.LastName;
            Email = manageUser.Email;
            IsCompetentAuthorityUser = manageUser.IsCompetentAuthorityUser;
            UserStatus = manageUser.UserStatus;
            UserStatusSelectList = new SelectList(EnumHelper.GetValues(typeof(UserStatus)), "Key", "Value");
            CanManageStatus = manageUser.CanManageStatus;
        }
    }
}
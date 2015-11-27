namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.Organisations;
    using Core.Shared;
    using System;
    using Web.ViewModels.Shared;

    public class OrganisationUserViewModel : RadioButtonGenericStringCollectionViewModel<UserStatus>
    {
        public Guid OrganisationUserId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public UserStatus UserStatus { get; set; }

        public OrganisationUserViewModel()
        {
        }

        public OrganisationUserViewModel(OrganisationUserData orgUser)
        {
            OrganisationUserId = orgUser.Id;
            UserId = orgUser.UserId;
            Firstname = orgUser.User.FirstName;
            Lastname = orgUser.User.Surname;
            Username = orgUser.User.Email;
            UserStatus = orgUser.UserStatus;
        }
    }
}
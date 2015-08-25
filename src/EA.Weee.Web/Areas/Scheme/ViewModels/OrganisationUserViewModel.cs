﻿namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using Core.Organisations;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class OrganisationUserViewModel
    {
        public RadioButtonStringCollectionViewModel UserStatuses { get; set; }

        public OrganisationUserViewModel()
        {
            UserStatuses = RadioButtonStringCollectionViewModel.CreateFromEnum<UserStatus>();
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public UserStatus UserStatus { get; set; }

        public OrganisationUserViewModel(OrganisationUserData orgUser)
        {
            UserId = orgUser.UserId;
            Firstname = orgUser.User.FirstName;
            Lastname = orgUser.User.Surname;
            Username = orgUser.User.Email;
            UserStatus = orgUser.UserStatus;
            UserStatuses = RadioButtonStringCollectionViewModel.CreateFromEnum<UserStatus>();
        }
    }
}
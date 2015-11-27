namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using EA.Weee.Core.Shared;

    public class UserAlreadyAssociatedWithOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public UserStatus Status { get; set; }
    }
}
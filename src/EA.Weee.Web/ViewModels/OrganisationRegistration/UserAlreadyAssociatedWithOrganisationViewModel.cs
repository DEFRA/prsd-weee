namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Shared;
    using System;

    public class UserAlreadyAssociatedWithOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public UserStatus Status { get; set; }
    }
}
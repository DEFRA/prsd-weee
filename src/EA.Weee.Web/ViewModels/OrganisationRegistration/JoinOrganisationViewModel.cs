namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class JoinOrganisationViewModel
    {
        [Required]
        public Guid OrganisationId { get; set; }

        [Required]
        public RadioButtonStringCollectionViewModel JoinOrganisationOptions { get; set; }
    }
}
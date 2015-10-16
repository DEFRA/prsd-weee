namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class JoinOrganisationViewModel : RadioButtonStringCollectionViewModel
    {
        [Required]
        public Guid OrganisationId { get; set; }
    }
}
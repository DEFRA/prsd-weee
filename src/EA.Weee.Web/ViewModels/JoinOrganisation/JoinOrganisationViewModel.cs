namespace EA.Weee.Web.ViewModels.JoinOrganisation
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
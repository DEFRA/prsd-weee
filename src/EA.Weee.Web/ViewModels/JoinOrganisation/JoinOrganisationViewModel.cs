namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class JoinOrganisationViewModel
    {
        [Required]
        public Guid OrganisationId { get; set; }
    }
}
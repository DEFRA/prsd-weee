namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class JoinOrganisationViewModel
    {
        [Required]
        public Guid OrganisationToJoin { get; set; }
    }
}
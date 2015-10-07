namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public string SearchedText { get; set; }
   
        [Required]
        public StringGuidRadioButtons Organisations { get; set; }
    }
}
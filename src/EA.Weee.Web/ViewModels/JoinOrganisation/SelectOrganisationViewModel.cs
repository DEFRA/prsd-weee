namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System.ComponentModel.DataAnnotations;
 
    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public string SearchedText { get; set; }
   
        [Required]
        public SelectOrganisationRadioButtons Organisations { get; set; }
    }
}
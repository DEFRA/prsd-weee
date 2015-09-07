namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System.ComponentModel.DataAnnotations;

    public class JoinOrganisationConfirmationViewModel
    {
        public JoinOrganisationConfirmationViewModel()
        {   
        }

        [Required]
        public string OrganisationName { get; set; }
    }
}
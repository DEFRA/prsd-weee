namespace EA.Weee.Web.ViewModels.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RepresentingCompaniesViewModel 
    {
        public Guid OrganisationId { get; set; }

        [DisplayName("Which represented organisation would you like to perform activities for?")]
        [Required(ErrorMessage = "Select a represented organisation to perform activities for")]
        public Guid? SelectedDirectRegistrant { get; set; }

        public IList<RepresentingCompany> Organisations { get; set; }

        public bool ShowBackButton { get; set; }
    }
}
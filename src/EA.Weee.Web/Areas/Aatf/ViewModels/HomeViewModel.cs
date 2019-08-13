namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class HomeViewModel
    {
        public Guid OrganisationId { get; set; }

        public bool IsAE { get; set; }

        [DisplayName("Which organisation would you like to perform activities for?")]
        [Required(ErrorMessage = "Select an organisation to perform activities")]
        public Guid? SelectedAatfId { get; set; }

        public IReadOnlyList<AatfData> AatfList { get; set; }

        public HomeViewModel()
        {
        }
    }
}
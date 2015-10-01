namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        public string SearchTerm { get; set; }

        public string SelectedRegistrationNumber { get; set; }
        public int? SelectedComplianceYear { get; set; }
    }
}
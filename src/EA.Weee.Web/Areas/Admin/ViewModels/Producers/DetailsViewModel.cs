namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using EA.Weee.Core.Admin;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class DetailsViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [Required]
        [DisplayName("Producer registration number (PRN)")]
        public string RegistrationNumber { get; set; }

        public ProducerDetails Details { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public DetailsViewModel()
        {
            Details = new ProducerDetails();
        }
    }
}
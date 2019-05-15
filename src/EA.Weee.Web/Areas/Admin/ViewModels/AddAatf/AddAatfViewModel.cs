namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using System.ComponentModel.DataAnnotations;
    public class AddAatfViewModel
    {
        [Required]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeName)]
        [DataType(DataType.Text)]
        [Display(Name = "Name of Aatf")]
        public string AatfName { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

        [Required]
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF",
            ErrorMessage = "Approval number is not in correct format.")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.SchemeApprovalNumber)]
        [DataType(DataType.Text)]
        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AatfStatus Status { get; set; }

        [Required]
        [Display(Name = "Size")]
        public AatfSize Size { get; set; }

        public AatfContactData ContactData { get; set; }
    }
}
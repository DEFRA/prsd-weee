namespace EA.Weee.Web.Areas.Test.ViewModels.CreatePcsDataReturnXmlFile
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataReturns;

    public class SpecifyOptionsViewModel
    {
        [Required]
        public Guid OrganisationID { get; set; }

        [Required]
        [Range(2016, 2099, ErrorMessage = "The compliance year must be between 2016 and 2099.")]
        [DisplayName("Compliance Year")]
        public int ComplianceYear { get; set; }

        [Required]
        [DisplayName("Quarter")]
        public QuarterType Quarter { get; set; }

        // TODO: Add options.

        public SpecifyOptionsViewModel()
        {
            // If acceptable, default to the current year.
            int year = DateTime.UtcNow.Year;
            
            if (year < 2016)
            {
                year = 2016;
            }

            if (year > 2099)
            {
                year = 2099;
            }

            ComplianceYear = year;
            Quarter = QuarterType.Q1;
        }
    }
}
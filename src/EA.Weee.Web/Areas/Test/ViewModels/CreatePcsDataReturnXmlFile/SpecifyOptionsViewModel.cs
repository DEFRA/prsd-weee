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

        [Required]
        [Range(0, 10000)]
        [DisplayName("Number of AATFs")]
        public int NumberOfAatfs{ get; set; }

        [Required]
        [Range(0, 10000)]
        [DisplayName("Number of AEs")]
        public int NumberOfAes { get; set; }

        [Required]
        [DisplayName("Producers")]
        public bool AllProducers { get; set; }

        [Required]
        [Range(0, 10000)]
        [DisplayName("Number of Producers")]
        public int NumberOfProducers { get; set; }

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
            AllProducers = true;
        }
    }
}
namespace EA.Weee.Web.Areas.Test.ViewModels
{
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
using System.Web.Mvc;

    public class GeneratePcsXmlOptionsViewModel
    {
        [Required]
        public Guid OrganisationID { get; set; }

        [Required]
        [DisplayName("Schema Version")]
        public SchemaVersion SchemaVersion { get; set; }

        [Required]
        [Range(2000, 2099, ErrorMessage = "The compliance year must be a year in the 21st century.")]
        [DisplayName("Compliance Year")]
        public int ComplianceYear { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The number of new producers must not be negative.")]
        [DisplayName("Number of new producers")]
        public int NumberOfNewProducers { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The number of existing producers must not be negative.")]
        [DisplayName("Number of existing producers")]
        public int NumberOfExistingProducers { get; set; }

        public GeneratePcsXmlOptionsViewModel()
        {
            // Default to the latest schema version.
            Array schemaVersions = Enum.GetValues(typeof(SchemaVersion));
            SchemaVersion = (SchemaVersion)schemaVersions.GetValue(schemaVersions.Length - 1);

            // Default to the current year.
            ComplianceYear = DateTime.UtcNow.Year;
        }
    }
}
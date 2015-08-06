﻿namespace EA.Weee.Web.Areas.Test.ViewModels.GeneratePcsXml
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
using System.Web.Mvc;
    using Core.Scheme.MemberUploadTesting;

    public class SpecifyOptionsViewModel
    {
        [Required]
        public Guid OrganisationID { get; set; }

        [Required]
        [DisplayName("Schema Version")]
        public SchemaVersion SchemaVersion { get; set; }

        [Required]
        [Range(2016, 2099, ErrorMessage = "The compliance year must be between 2016 and 2099.")]
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

        [DisplayName("Include badly-formed root element")]
        public bool IncludeMalformedSchema { get; set; }

        [DisplayName("Include unexpeted &lt;foo/&gt; element")]
        public bool IncludeUnexpectedFooElement { get; set; }

        [DisplayName("Ignore string length conditions for all producers")]
        public bool IgnoreStringLengthConditions { get; set; }

        public SpecifyOptionsViewModel()
        {
            // Default to the latest schema version.
            Array schemaVersions = Enum.GetValues(typeof(SchemaVersion));
            SchemaVersion = (SchemaVersion)schemaVersions.GetValue(schemaVersions.Length - 1);

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
        }
    }
}
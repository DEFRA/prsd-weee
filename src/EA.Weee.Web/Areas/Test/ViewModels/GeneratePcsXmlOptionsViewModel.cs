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
        [Range(0, int.MaxValue, ErrorMessage = "The number of new producers must not be negative.")]
        [DisplayName("Number of new producers")]
        public int NumberOfNewProducers { get; set; }
    }
}
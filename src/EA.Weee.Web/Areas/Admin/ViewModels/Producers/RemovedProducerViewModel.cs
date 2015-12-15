namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System;

    public class RemovedProducerViewModel
    {
        public Guid RegisteredProducerId { get; set; }

        public string RegistrationNumber { get; set; }

        public int ComplianceYear { get; set; }

        public string SchemeName { get; set; }
    }
}
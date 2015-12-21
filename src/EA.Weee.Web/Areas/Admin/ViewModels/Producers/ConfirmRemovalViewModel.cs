namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRemovalViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please confirm")]
        public override string SelectedValue { get; set; }

        public Guid RegisteredProducerId { get; set; }

        public string RegistrationNumber { get; set; }

        public int ComplianceYear { get; set; }

        public string SchemeName { get; set; }

        public string ProducerName { get; set; }
    }
}
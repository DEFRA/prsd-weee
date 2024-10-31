namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using Core.Admin;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmReturnViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please confirm")]
        public override string SelectedValue { get; set; }

        public ProducerDetailsScheme Producer { get; set; }

        public Guid DirectProducerSubmissionId { get; set; }
    }
}
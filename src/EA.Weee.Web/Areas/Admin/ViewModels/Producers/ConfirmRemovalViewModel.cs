namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Admin;
    using Web.ViewModels.Shared;

    public class ConfirmRemovalViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please confirm")]
        public override string SelectedValue { get; set; }

        public ProducerDetailsScheme Producer { get; set; }
    }
}
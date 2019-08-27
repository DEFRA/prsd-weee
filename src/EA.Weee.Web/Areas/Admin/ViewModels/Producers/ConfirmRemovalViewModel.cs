namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using Core.Admin;
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRemovalViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please confirm")]
        public override string SelectedValue { get; set; }

        public ProducerDetailsScheme Producer { get; set; }
    }
}
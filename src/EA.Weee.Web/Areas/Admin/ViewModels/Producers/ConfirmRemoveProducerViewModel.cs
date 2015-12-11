namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class ConfirmRemoveProducerViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please confirm")]
        public override string SelectedValue { get; set; }

        public string RegistrationNumber { get; set; }
    }
}
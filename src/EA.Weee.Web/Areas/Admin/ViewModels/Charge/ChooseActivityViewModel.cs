namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ChooseActivityViewModel
    {
        [Required(ErrorMessage = "Select an activity")]
        [Display(Name = "Select the activity you would like to do?")]
        public Activity? SelectedActivity { get; set; }

        public IReadOnlyCollection<Activity> PossibleValues { get; private set; }

        public ChooseActivityViewModel()
        {
            PossibleValues = new List<Activity>()
            {
                Activity.ManagePendingCharges,
                Activity.ManageIssuedCharges,
                Activity.ViewInvoiceRunHistory
            };
        }
    }
}
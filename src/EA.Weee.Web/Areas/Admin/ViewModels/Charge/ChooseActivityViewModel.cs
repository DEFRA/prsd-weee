namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.Shared;

    public class ChooseActivityViewModel
    {
        [Required(ErrorMessage = "Select an activity")]
        [Display(Name = "What would you like to do?")]
        public Activity? SelectedActivity { get; set; }

        public IReadOnlyCollection<Activity> PossibleValues { get; private set; }

        public ChooseActivityViewModel()
        {
            PossibleValues = new List<Activity>()
            {
                Activity.ManagePendingCharges,
                //Activity.ManageIssuedCharges,
                Activity.ViewInvoiceRunHistory
            };
        }
    }
}
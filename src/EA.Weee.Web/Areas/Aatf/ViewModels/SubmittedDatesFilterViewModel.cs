namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel;

    public class SubmittedDatesFilterViewModel
    {
        [DisplayName("Submitted date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Submitted date")]
        public DateTime? EndDate { get; set; }

        public bool SearchOnDatesPerformed
        {
            get
            {
                return true;
            }
        }
    }
}
namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class SelectReportOptionsDeselectViewModel : SelectReportOptionsModelBase
    {
        public SelectReportOptionsDeselectViewModel()
        {
        }

        [Required(ErrorMessage = "You must tell us if you want to remove these reporting options")]
        public virtual string SelectedValue { get; set; }
    }
}